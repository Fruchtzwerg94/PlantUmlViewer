using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlantUmlViewer.Helpers
{
    /// <summary>
    /// Helper class to work with <see cref="Process"/>es
    /// </summary>
    internal static class ProcessHelper
    {
        /// <summary>
        /// Opens a document using the systems associated application
        /// </summary>
        /// <param name="fileName">The name of the document to open</param>
        /// <exception cref="InvalidOperationException">Failed to open file due to non 0 return value</exception>
        public static void OpenDocument(string fileName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = fileName,
                UseShellExecute = true
            };
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException("Failed to open file with code " + process.ExitCode);
                }
            }
        }

        /// <summary>
        /// Asynchronously creates and runs a process
        /// </summary>
        /// <param name="fileName">The name of the application to start, or the name of a document of a file type</param>
        /// <param name="arguments">Command-line arguments to pass to the application when the process starts</param>
        /// <param name="workingDirectory">The working directory for the process to be started</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel execution and kill the process</param>
        /// <param name="input">Data to write to the standard input of the process</param>
        /// <returns>A task running the process and providing its result</returns>
        public static async Task<ProcessResult> RunProcessAsync(string fileName, IEnumerable<string> arguments,
            string workingDirectory, byte[] input = null, CancellationToken cancellationToken = default)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName,
                string.Join(" ", arguments.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => $"{a}")))
            {
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };
            Debug.WriteLine(startInfo.Arguments);

            using (Process process = new Process() { StartInfo = startInfo })
            {
                return await process.RunAsync(input, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
