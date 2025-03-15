using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PlantUmlViewer.Helpers
{
    /// <summary>
    /// Process result data
    /// </summary>
    internal class ProcessResult
    {
        /// <summary>
        /// The <see cref="Process.StandardOutput"/> read to an array of bytes
        /// </summary>
        public byte[] Output { get; }

        /// <summary>
        /// The <see cref="Process.StandardError"/> read to an array of bytes
        /// </summary>
        public byte[] Error { get; }

        /// <summary>
        /// The code that the associated process specified when it terminated
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// Initializes a new instance of the ProcessResult class
        /// </summary>
        /// <param name="output">The <see cref="Process.StandardOutput"/> read to an array of bytes</param>
        /// <param name="error">The <see cref="Process.StandardError"/> read to an array of bytes</param>
        /// <param name="exitCode">The code that the associated process specified when it terminated</param>
        public ProcessResult(byte[] output, byte[] error, int exitCode)
        {
            Output = output;
            Error = error;
            ExitCode = exitCode;
        }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="Process"/>
    /// </summary>
    internal static class ProcessExtensions
    {
        /// <summary>
        /// Asynchronously runs a process
        /// </summary>
        /// <param name="process">The process to run</param>
        /// <param name="input">Data to write to the standard input of the process</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel execution and kill the process</param>
        /// <returns>A task running the process and providing its result</returns>
        /// <exception cref="InvalidOperationException">Failed to start process</exception>
        public static async Task<ProcessResult> RunAsync(this Process process, byte[] input = null,
            CancellationToken cancellationToken = default)
        {
            process.EnableRaisingEvents = true;

            TaskCompletionSource<ProcessResult> tcs = new TaskCompletionSource<ProcessResult>();
            if (cancellationToken != CancellationToken.None)
            {
                cancellationToken.Register(() =>
                {
                    if (tcs.TrySetCanceled())
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {
                            //Nothing else we can do here
                        }
                    }
                });
            }

            if (!process.Start())
            {
                throw new InvalidOperationException("Failed to start process " + process);
            }

            if (input != null)
            {
                await process.StandardInput.BaseStream
                    .WriteAsync(input, 0, input.Length, cancellationToken).ConfigureAwait(false);
                await process.StandardInput.FlushAsync().ConfigureAwait(false);
                process.StandardInput.Close();
            }

            _ = Task.Run(() =>
            {
                ProcessResult result = new ProcessResult(
                    process.GetOutput(), process.GetError(), process.ExitCode);
                tcs.SetResult(result);
            }, CancellationToken.None);

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the <see cref="Process.StandardOutput"/> to an array
        /// </summary>
        /// <param name="process"></param>
        /// <returns>The <see cref="Process.StandardOutput"/> read to an array of bytes</returns>
        public static byte[] GetOutput(this Process process)
        {
            return ExtractBytes(process.StandardOutput.BaseStream);
        }

        /// <summary>
        /// Reads the <see cref="Process.StandardError"/> to an array
        /// </summary>
        /// <param name="process"></param>
        /// <returns>The <see cref="Process.StandardError"/> read to an array of bytes</returns>
        public static byte[] GetError(this Process process)
        {
            return ExtractBytes(process.StandardError.BaseStream);
        }

        private static byte[] ExtractBytes(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
