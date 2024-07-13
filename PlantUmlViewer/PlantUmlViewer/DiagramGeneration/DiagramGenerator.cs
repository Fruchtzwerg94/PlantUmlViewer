using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Svg;

using PlantUmlViewer.DiagramGeneration.PlantUml;

namespace PlantUmlViewer.DiagramGeneration
{
    internal static class DiagramGenerator
    {
        private const string DIAGRAM_DELIMITOR = "|##|##|<PS>|##|##|";
        private const string SVG_START = "<svg ";
        private const string SVG_END = "</svg>";

        public static async Task<List<GeneratedDiagram>> GenerateDocumentAsync(
            string javaExecutable, string plantUmlJar,
            string text, string include, string workingDirectory,
            CancellationTokenSource cancellationTokenSource)
        {
            /*
             * The PlantUML responses could contain multiple pages, where each page could contain the images for multiple diagrams
             * The dictionaries are build up like, note that some images in between may are not set since they are empty
             *  - Page[0]
             *      - DiagramImage[0]
             *      - ...
             *      - DiagramImage[n]
             *  - ...
             *      - ...
             *  - Page[n]
             *      - ...
             *
             * Diagrams can contain multiple pages, which are generated in independent steps
             * At least 2 generation calls needed to be done for each diagram to find out this was the last page
             * To speed this up, the first two calls are executed parallel
             * Afterwards page by page is generated
             */

            Dictionary<int, Dictionary<int, SvgDocument>> pages = new Dictionary<int, Dictionary<int, SvgDocument>>();

            //Generate the first page directly at startup
            List<Task<bool>> generateTasks = new List<Task<bool>>()
            {
                Task.Run(() => GeneratePageAsync(javaExecutable, plantUmlJar,
                  text, include, workingDirectory, 0, pages, cancellationTokenSource))
            };
            //Generate the (maybe) following pages
            int pageIndex = 1;
            while (true)
            {
                generateTasks.Add(Task.Run(() => GeneratePageAsync(javaExecutable, plantUmlJar,
                    text, include, workingDirectory, pageIndex, pages, cancellationTokenSource)));
                await Task.WhenAll(generateTasks);
                if (generateTasks.Exists(rT => !rT.Result))
                {
                    //The last page was detected
                    break;
                }
                generateTasks.Clear();
                pageIndex++;
            }

            Debug.WriteLine($"{pages.Sum(p => p.Value.Count)} images(s) at {pages.Count} page(s) generated",
                nameof(DiagramGenerator));

            return ReorganizePagesToDiagram(pages);
        }

        public static Image SvgImageToBitmap(SvgDocument svgImage, decimal sizeFactor)
        {
            //Resize (see: https://github.com/svg-net/SVG/blob/master/Source/SvgDocument.Drawing.cs#L217)
            SizeF svgSize = svgImage.GetDimensions();
            SizeF imageSize = svgSize;
            svgImage.RasterizeDimensions(ref imageSize,
                (int)Math.Round((decimal)svgSize.Width * sizeFactor), (int)Math.Round((decimal)svgSize.Height * sizeFactor));
            Size bitmapSize = Size.Round(imageSize);
            Bitmap image = new Bitmap(bitmapSize.Width, bitmapSize.Height);

            //Set background if defined in SVG style since PlantUml does not set it e.g. if white
            if (svgImage.TryGetAttribute(ExCSS.PropertyNames.BackgroundColor, out string backgroundAttribute))
            {
                using (Graphics g = Graphics.FromImage(image))
                using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml(backgroundAttribute)))
                {
                    g.FillRectangle(brush, 0, 0, image.Width, image.Height);
                }
            }

            //Render
            using (ISvgRenderer renderer = SvgRenderer.FromImage(image))
            {
                renderer.ScaleTransform(imageSize.Width / svgSize.Width, imageSize.Height / svgSize.Height);
                svgImage.Draw(renderer);
            }
            return image;
        }

        private static async Task<bool> GeneratePageAsync(string javaExecutable, string plantUmlJar,
            string text, string include, string workingDirectory, int pageIndexToGenerate,
            Dictionary<int, Dictionary<int, SvgDocument>> pages, CancellationTokenSource cancellationTokenSource)
        {
            PlantUmlArguments arguments = new PlantUmlArguments()
            {
                OutputFormat = OutputFormat.Svg,
                ErrorFormat = ErrorFormat.Verbose,
                Include = include,
                WorkingDirectory = workingDirectory,
                Delimitor = DIAGRAM_DELIMITOR,
                ImageIndex = pageIndexToGenerate
            };

            byte[] bytes = await PlantUmlRunner.Generate(javaExecutable, plantUmlJar, arguments, workingDirectory, text,
                cancellationTokenSource.Token).ConfigureAwait(true);

            //Find all delimitors to parse multiple diagram images
            Dictionary<int, SvgDocument> imagesOfPage = new Dictionary<int, SvgDocument>();
            List<int> delimitorIndices = new int[] { -(DIAGRAM_DELIMITOR.Length + 2) }
                .Concat(PatternAt(bytes, Encoding.UTF8.GetBytes(DIAGRAM_DELIMITOR), 0, bytes.Length)).ToList();
            for (int i = 0; i < delimitorIndices.Count - 1; i++)
            {
                int start = delimitorIndices[i] + DIAGRAM_DELIMITOR.Length + 2;
                int end = delimitorIndices[i + 1];
                if (end - start > 0)
                {
                    //Remove all unexpected data which may is added due to Java accessibility hooks output e.g. like PowerAutomate
                    List<int> startPatterns = PatternAt(bytes, Encoding.UTF8.GetBytes(SVG_START), start, end).ToList();
                    List<int> endPatterns = PatternAt(bytes, Encoding.UTF8.GetBytes(SVG_END), start, end).ToList();
                    if (startPatterns.Count != 1 || endPatterns.Count != 1)
                    {
                        throw new InvalidOperationException("Failed to parse generated data");
                    }
                    start = startPatterns[0];
                    end = endPatterns[0] + SVG_END.Length;

                    using (MemoryStream memoryStream = new MemoryStream(bytes, start, end - start))
                    {
                        Debug.WriteLine($"Generating image {i + 1} at page {pageIndexToGenerate + 1}", nameof(DiagramGenerator));
                        imagesOfPage[i] = SvgDocument.Open<SvgDocument>(memoryStream);
                    }
                }
                else
                {
                    Debug.WriteLine($"No image {i + 1} at page {pageIndexToGenerate + 1}", nameof(DiagramGenerator));
                }
            }
            //No more pages available
            if (imagesOfPage.Count == 0)
            {
                return false;
            }
            pages[pageIndexToGenerate] = imagesOfPage;
            return true;
        }

        private static List<GeneratedDiagram> ReorganizePagesToDiagram(Dictionary<int, Dictionary<int, SvgDocument>> pages)
        {
            //Reorganize page based structure to intuitive diagram based structure
            //pages[pageIndex][diagramIndex] --> images[diagramIndex][nonEmptyPageIndex]
            int numberOfDiagrams = pages.Max(p => p.Value.Max(d => d.Key)) + 1;
            List<GeneratedDiagram> images = Enumerable.Repeat<object>(null, numberOfDiagrams)
                .Select(_ => new GeneratedDiagram()).ToList();
            for (int diagramIndex = 0; diagramIndex < numberOfDiagrams; diagramIndex++)
            {
                foreach (Dictionary<int, SvgDocument> newPage in pages.OrderBy(nP => nP.Key).Select(nP => nP.Value))
                {
                    if (newPage.TryGetValue(diagramIndex, out SvgDocument newDiagram))
                    {
                        images[diagramIndex].Pages.Add(newDiagram);
                    }
                }
            }
            return images;
        }

        private static IEnumerable<int> PatternAt(byte[] source, byte[] pattern, int start, int end)
        {
            if (source == null || pattern == null || source.Length < pattern.Length)
            {
                yield break;
            }

            end = Math.Min(source.Length, end);
            for (int i = start; i < end; i++)
            {
                if (IsPatternMatch(source, i, pattern))
                {
                    yield return i;
                }
            }
        }

        private static bool IsPatternMatch(byte[] source, int position, byte[] pattern)
        {
            if (pattern.Length > (source.Length - position))
            {
                return false;
            }
            for (int i = 0; i < pattern.Length; i++)
            {
                if (source[position + i] != pattern[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
