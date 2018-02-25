using System.Diagnostics;

namespace CoreDirectivesGenerator
{
    /// <summary>
    /// Run shell commands on Windows or Unix.
    /// Source: https://stackoverflow.com/a/45338239
    /// </summary>
    public static class Shell
    {
        public static ProcessResult Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            return Run("/bin/bash", $"-c \"{escapedArgs}\"");
        }

        public static ProcessResult Bat(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            return Run("cmd.exe", $"/c \"{escapedArgs}\"");
        }

        private static ProcessResult Run(string filename, string arguments)
        {
            var result = new ProcessResult();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };
            process.Start();
            result.StdOut = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            result.ExitCode = process.ExitCode;

            return result;
        }
    }

    public class ProcessResult {
        public string StdOut { get; set; }
        public int? ExitCode { get; set; }

        public bool WasSuccessful() {
            return ExitCode == 0;
        }
    }
}
