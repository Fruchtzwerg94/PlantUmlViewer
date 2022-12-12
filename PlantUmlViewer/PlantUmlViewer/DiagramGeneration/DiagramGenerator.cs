using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Svg;

using PlantUml.Net;

namespace PlantUmlViewer.DiagramGeneration
{
    public class DiagramGenerator
    {
        private const string DIAGRAM_DELIMITOR = "|##|##|<PS>|##|##|";

        private readonly RendererFactory renderFactory = new RendererFactory();
        public string JavaPath { get; set; }
        public string PlantUmlBinary { get; set; }

        public DiagramGenerator(string javaPath, string plantUmlBinary)
        {
            JavaPath = javaPath;
            PlantUmlBinary = plantUmlBinary;
        }

        public async Task<List<GeneratedDiagram>> GenerateDocumentAsync(string text, string include, CancellationTokenSource cancellationTokenSource)
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
                Task.Run(() => GeneratePageAsync(text, include, 0, pages, cancellationTokenSource))
            };
            //Generate the (maybe) following pages
            int pageIndex = 1;
            while (true)
            {
                generateTasks.Add(Task.Run(() => GeneratePageAsync(text, include, pageIndex, pages, cancellationTokenSource)));
                await Task.WhenAll(generateTasks);
                if (generateTasks.Any(rT => !rT.Result))
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

        private async Task<bool> GeneratePageAsync(string text, string include, int pageIndexToGenerate,
            Dictionary<int, Dictionary<int, SvgDocument>> pages, CancellationTokenSource cancellationTokenSource)
        {
            IPlantUmlRenderer renderer = renderFactory.CreateRenderer(new PlantUmlSettings()
            {
                ErrorReportMode = ErrorReportMode.Verbose,
                LocalPlantUmlPath = PlantUmlBinary,
                JavaPath = JavaPath,
                RenderingMode = RenderingMode.Local,
                Include = include,
                Delimitor = DIAGRAM_DELIMITOR,
                ImageIndex = pageIndexToGenerate
            });

            byte[] bytes = await renderer.RenderAsync(text, OutputFormat.Svg,
                cancellationTokenSource.Token).ConfigureAwait(true);

            //Find all delimitors to parse multiple diagram images
            Dictionary<int, SvgDocument> imagesOfPage = new Dictionary<int, SvgDocument>();
            List<int> delimitorIndices = new int[] { -(DIAGRAM_DELIMITOR.Length + 2) }
                .Concat(PatternAt(bytes, Encoding.UTF8.GetBytes(DIAGRAM_DELIMITOR))).ToList();
            for (int i = 0; i < delimitorIndices.Count - 1; i++)
            {
                int start = delimitorIndices[i] + DIAGRAM_DELIMITOR.Length + 2;
                int end = delimitorIndices[i + 1];
                using (MemoryStream memoryStream = new MemoryStream(bytes, start, end - start))
                {
                    if (end - start > 0)
                    {
                        Debug.WriteLine($"Generating image {i + 1} at page {pageIndexToGenerate + 1}", nameof(DiagramGenerator));
                        imagesOfPage[i] = SvgDocument.Open<SvgDocument>(memoryStream);
                    }
                    else
                    {
                        Debug.WriteLine($"No image {i + 1} at page {pageIndexToGenerate + 1}", nameof(DiagramGenerator));
                    }
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

        private List<GeneratedDiagram> ReorganizePagesToDiagram(Dictionary<int, Dictionary<int, SvgDocument>> pages)
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

        private static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }
    }
}
