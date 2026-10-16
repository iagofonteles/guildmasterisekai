using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodexUnityPlugin.Editor
{
    internal sealed class CodexProcess
    {
        internal sealed class Result
        {
            public int ExitCode;
            public string Output;
            public string Error;

            public string RequireSuccess()
            {
                if (ExitCode != 0)
                    throw new IOException($"Command failed (exit {ExitCode}).\n{Error}\n{Output}");
                return Output;
            }
        }

        // One exec process per request. An idle integration has no OS process.
        public async Task<Result> Run(string executable, string arguments, string root,
            string input, CancellationToken token, int timeoutSeconds = 0)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(executable, arguments)
                {
                    WorkingDirectory = root,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };
                token.ThrowIfCancellationRequested();
                if (!process.Start()) throw new IOException("Could not start " + executable);
                using (var cancellation = token.Register(() =>
                {
                    try
                    {
                        if (!process.HasExited)
                            Process.Start(new ProcessStartInfo("taskkill.exe", "/PID " + process.Id + " /T /F")
                                { UseShellExecute = false, CreateNoWindow = true })?.Dispose();
                    }
                    catch (InvalidOperationException) { }
                    catch (System.ComponentModel.Win32Exception) { }
                }))
                {
                    var output = process.StandardOutput.ReadToEndAsync();
                    var error = process.StandardError.ReadToEndAsync();
                    var started = DateTime.UtcNow;
                    try
                    {
                        if (input != null) await process.StandardInput.WriteAsync(input).ConfigureAwait(false);
                        process.StandardInput.Close();
                        while (!process.HasExited)
                        {
                            token.ThrowIfCancellationRequested();
                            if (timeoutSeconds > 0 && (DateTime.UtcNow - started).TotalSeconds > timeoutSeconds)
                                throw new TimeoutException("CLI validation timed out.");
                            await Task.Delay(100, token).ConfigureAwait(false);
                        }
                        token.ThrowIfCancellationRequested();
                        return new Result { ExitCode = process.ExitCode,
                            Output = await output.ConfigureAwait(false), Error = await error.ConfigureAwait(false) };
                    }
                    finally
                    {
                        if (!process.HasExited)
                        {
                            // Unity's .NET profile lacks Process.Kill(entireProcessTree).
                            // taskkill receives only the numeric PID; no user text enters a shell.
                            using (var killer = Process.Start(new ProcessStartInfo("taskkill.exe",
                                       "/PID " + process.Id + " /T /F") { UseShellExecute = false, CreateNoWindow = true }))
                            {
                                if (killer != null) await Task.Run(() => killer.WaitForExit()).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }

        public static string ResolveExecutable(string configured)
        {
            if (!string.IsNullOrWhiteSpace(configured))
            {
                if (!Path.IsPathRooted(configured) || !File.Exists(configured) ||
                    !string.Equals(Path.GetExtension(configured), ".exe", StringComparison.OrdinalIgnoreCase))
                    throw new IOException("Select an existing absolute path to codex.exe.");
                return configured;
            }
            foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
            {
                var candidate = Path.Combine(directory.Trim().Trim('"'), "codex.exe");
                if (File.Exists(candidate)) return candidate;
            }
            throw new FileNotFoundException("codex.exe is missing from Unity's PATH. Install Codex CLI or set its absolute executable path, then run codex login in a terminal.");
        }
    }
}
