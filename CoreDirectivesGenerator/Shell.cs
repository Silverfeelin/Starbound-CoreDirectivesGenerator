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
            return Bash(cmd, new ProcessOptions());
        }
        public static ProcessResult Bash(this string cmd, ProcessOptions opts)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            return Run("/bin/bash", $"-c \"{escapedArgs}\"", opts);
        }

        public static ProcessResult Bat(this string cmd)
        {
            return Bat(cmd, new ProcessOptions());
        }
        public static ProcessResult Bat(this string cmd, ProcessOptions opts)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            return Run("cmd.exe", $"/c \"{escapedArgs}\"", opts);
        }

        private static ProcessResult Run(string filename, string arguments, ProcessOptions opts)
        {
            var result = new ProcessResult();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = arguments,
                    RedirectStandardOutput = opts.ShouldRedirectStdOut,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };
            process.Start();

            if (process.StartInfo.RedirectStandardOutput)
            {
                result.StdOut = process.StandardOutput.ReadToEnd();
            }
            
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

    public class ProcessOptions {
        public bool ShouldRedirectStdOut { get; set; } = true;
    }
}
