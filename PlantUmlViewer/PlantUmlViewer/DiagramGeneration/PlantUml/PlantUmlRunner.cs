using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PlantUmlViewer.Helpers;

namespace PlantUmlViewer.DiagramGeneration.PlantUml
{
    internal static class PlantUmlRunner
    {
        public static async Task<byte[]> Generate(string javaExecutable, string plantUmlJar,
            PlantUmlArguments plantUmlArguments, string workingDirectory, string code,
            CancellationToken cancellationToken = default)
        {
            if (!File.Exists(javaExecutable))
            {
                throw new FileNotFoundException($"Java executable '{javaExecutable}' does not exist");
            }
            if (!File.Exists(plantUmlJar))
            {
                throw new FileNotFoundException($"PlantUML JAR '{plantUmlJar}' does not exist");
            }

            ProcessResult result = await ProcessHelper.RunProcessAsync(javaExecutable,
                new string[]
                {
                    "-Dfile.encoding=UTF-8",
                    "-jar",
                    $"\"{plantUmlJar}\"",
                    "-pipe",
                    "-charset UTF-8",
                    GetOutputFormatArgument(plantUmlArguments.OutputFormat),
                    GetErrorFormatArgument(plantUmlArguments.ErrorFormat),
                    string.IsNullOrEmpty(plantUmlArguments.FileName) ? string.Empty : $"-filename \"{plantUmlArguments.FileName}\"",
                    string.IsNullOrEmpty(plantUmlArguments.Include) ? string.Empty : $"\"-I{plantUmlArguments.Include}\"",
                    string.IsNullOrEmpty(plantUmlArguments.Delimitor) ? string.Empty : $"-pipedelimitor \"{plantUmlArguments.Delimitor}\"",
                    $"-pipeimageindex {plantUmlArguments.ImageIndex}"
                },
                workingDirectory, Encoding.UTF8.GetBytes(code), cancellationToken);
            if (result.ExitCode != 0)
            {
                string message = Encoding.UTF8.GetString(result.Error);
                throw new RenderException(code, message);
            }
            return result.Output;
        }

        private static string GetOutputFormatArgument(OutputFormat outputFormat)
        {
            switch (outputFormat)
            {
                case OutputFormat.Svg:
                    return "-tsvg";
                case OutputFormat.Png:
                    return "-tpng";
                case OutputFormat.Eps:
                    return "-teps";
                case OutputFormat.Pdf:
                    return "-tpdf";
                case OutputFormat.Vdx:
                    return "-tvdx";
                case OutputFormat.Xmi:
                    return "-txmi";
                case OutputFormat.Scxml:
                    return "-tscxml";
                case OutputFormat.Html:
                    return "-thtml";
                case OutputFormat.Ascii:
                    return "-ttxt";
                case OutputFormat.AsciiUnicode:
                    return "-tutxt";
                case OutputFormat.LaTeX:
                    return "-tlatex";
                default:
                    throw new NotImplementedException($"Invalid {nameof(OutputFormat)}");
            }
        }

        private static string GetErrorFormatArgument(ErrorFormat errorFormat)
        {
            switch (errorFormat)
            {
                case ErrorFormat.TwoLines:
                    return "";
                case ErrorFormat.SingleLine:
                    return "-stdrpt:2";
                case ErrorFormat.Verbose:
                    return "-stdrpt:1";
                default:
                    throw new NotImplementedException($"Invalid {nameof(ErrorFormat)}");
            }
        }
    }
}
