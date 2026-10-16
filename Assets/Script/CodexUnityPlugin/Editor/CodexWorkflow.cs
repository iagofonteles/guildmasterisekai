using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CodexUnityPlugin.Editor
{
    internal static class CodexWorkflow
    {
        public const string OutputFolder = "Assets/Script/CodexUnityPlugin/Reports";
        private const string Start = "### Unity Codex Tasks Workflow";
        private const string End = "### End Unity Codex Tasks Workflow";
        internal static string Section => Start + @"

- This is a Unity project. Respect its Unity version, assembly definitions, namespaces, project structure, and existing coding style.
- Prompts starting with [QuickAnswer] are read-only questions. Inspect code and referenced objects, but do not modify, create, rename, or delete project files. Return plain text. The Editor integration writes the answer to " + OutputFolder + @"/QuickAnswer.txt.
- Prompts starting with [TaskFile] are implementation tasks. Implement only the requested changes, preserve existing work, and follow the report format and exact path supplied by the Editor integration. Do not automatically commit.
- Reports must contain a summary, completed work, validation results, limitations, and a final section titled 'Modified assets / files:' listing every known added, modified, or deleted project-relative path. State clearly when the list cannot be verified. Never invent results.
- Referenced Unity objects are contextual targets, not edit permission. Resolve scene paths and hierarchy carefully; unsaved scene changes may be unavailable on disk.
- Treat " + OutputFolder + @"/ as temporary output. The Editor integration writes reports from the final response.
- Never assume success without checking tool/process results.

" + End;

        public static void UpdateAgents(string root)
        {
            var path = Path.Combine(root, "AGENTS.md");
            var text = File.Exists(path) ? File.ReadAllText(path) : "";
            var start = text.IndexOf(Start, StringComparison.Ordinal);
            var end = text.IndexOf(End, StringComparison.Ordinal);
            if ((start >= 0) != (end >= 0) || (start >= 0 && end < start))
                throw new IOException("AGENTS.md contains an incomplete workflow section. Repair its delimiters before initialization.");
            var updated = start < 0 ? text.TrimEnd() + "\n\n" + Section + "\n" :
                text.Substring(0, start) + Section + text.Substring(end + End.Length);
            if (updated != text) File.WriteAllText(path, updated);
        }

        public static async Task<string> Initialize(string root, string configured, CancellationToken token)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new PlatformNotSupportedException("This integration requires Windows and codex.exe.");
            Directory.CreateDirectory(Path.Combine(root, OutputFolder));
            UpdateAgents(root);
            var runner = new CodexProcess();
            // Walk parent directories: works for ordinary repositories and .git worktree files.
            var repository = new DirectoryInfo(root);
            while (repository != null && !Directory.Exists(Path.Combine(repository.FullName, ".git")) &&
                   !File.Exists(Path.Combine(repository.FullName, ".git"))) repository = repository.Parent;
            if (repository != null)
            {
                var relative = Path.GetRelativePath(repository.FullName, Path.Combine(root, OutputFolder)).Replace('\\', '/');
                var rule = "/" + relative + "/";
                var ignore = Path.Combine(repository.FullName, ".gitignore");
                var existing = File.Exists(ignore) ? File.ReadAllText(ignore) : "";
                if (!existing.Split('\n').Any(line => line.TrimEnd('\r') == rule))
                    File.AppendAllText(ignore, (existing.EndsWith("\n") || existing.Length == 0 ? "" : "\n") + rule + "\n");
            }
            var executable = CodexProcess.ResolveExecutable(configured);
            var help = (await runner.Run(executable, "exec --help", root, null, token, 20)).RequireSuccess();
            foreach (var flag in new[] { "--sandbox", "read-only", "workspace-write", "--color" })
                if (!help.Contains(flag)) throw new IOException("Installed Codex does not advertise required option: " + flag);
            (await runner.Run(executable, "login status", root, null, token, 20)).RequireSuccess();
            return executable;
        }

        public static string ReserveReport(string root, string question)
        {
            var title = Regex.Replace(question.Split(new[] { '\r', '\n' })[0], @"[^\p{L}\p{N} _-]", "").Trim();
            if (title.Length > 60) title = title.Substring(0, 60).TrimEnd();
            if (title.Length == 0) title = "Task";
            var folder = Path.Combine(root, OutputFolder);
            Directory.CreateDirectory(folder);
            var prefix = "Task " + DateTime.Now.ToString("MMdd") + "-";
            var maximum = Directory.GetFiles(folder, prefix + "*.txt").Select(Path.GetFileName)
                .Select(name => Regex.Match(name, @"^Task \d{4}-(\d+) "))
                .Where(match => match.Success).Select(match => int.Parse(match.Groups[1].Value)).DefaultIfEmpty(0).Max();
            for (var number = maximum + 1; ; number++)
            {
                var path = Path.Combine(folder, prefix + number.ToString("D3") + " " + title + ".txt");
                try { using (new FileStream(path, FileMode.CreateNew, FileAccess.Write)) { } return path; }
                catch (IOException) { if (!File.Exists(path)) throw; }
            }
        }
    }
}
