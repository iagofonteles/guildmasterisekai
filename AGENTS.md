# Task-file workflow

When the user says "do task" or a similar request to execute the task file, read `Assets/codex task.txt` again and follow its current instructions. Do not reuse the contents from a previous turn; the user may have edited the file.
After execunting task from the task file, append to the task file a list of modified files and a brief report the task with the found problems during execution

### Unity Codex Tasks Workflow

- This is a Unity project. Respect its Unity version, assembly definitions, namespaces, project structure, and existing coding style.
- Prompts starting with [QuickAnswer] are read-only questions. Inspect code and referenced objects, but do not modify, create, rename, or delete project files. Return plain text. The Editor integration writes the answer to Assets/Script/CodexUnityPlugin/Reports/QuickAnswer.txt.
- Prompts starting with [TaskFile] are implementation tasks. Implement only the requested changes, preserve existing work, and follow the report format and exact path supplied by the Editor integration. Do not automatically commit.
- Reports must contain a summary, completed work, validation results, limitations, and a final section titled 'Modified assets / files:' listing every known added, modified, or deleted project-relative path. State clearly when the list cannot be verified. Never invent results.
- Referenced Unity objects are contextual targets, not edit permission. Resolve scene paths and hierarchy carefully; unsaved scene changes may be unavailable on disk.
- Treat Assets/Script/CodexUnityPlugin/Reports/ as temporary output. The Editor integration writes reports from the final response.
- Never assume success without checking tool/process results.

### End Unity Codex Tasks Workflow
