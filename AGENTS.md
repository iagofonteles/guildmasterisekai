# Task-file workflow


### Unity Codex Tasks Workflow

- This is a Unity project. Respect its Unity version, assembly definitions, namespaces, project structure, and existing coding style.
- When the user says "execute {filename}" read the txt file and follow its current instructions. Do not reuse the contents from a previous turn; the user may have edited the file.
- When the user adds "codeonly" or similar to a request, make it a fast iteration. only edit text files, dont run builds or tests, dont try to validate outsite code context.
- After execunting task from the task file, append to the task file a list of modified files and a brief report the task with the found problems during execution
- Prompts starting with "question" or similar are read-only questions. Inspect code and referenced objects, but do not modify, create, rename, or delete project files. Return plain text.
- Reports must contain a summary, completed work, validation results, limitations, and a final section titled 'Modified assets / files:' listing every known added, modified, or deleted project-relative path. State clearly when the list cannot be verified. Never invent results.
- Referenced Unity objects are contextual targets, not edit permission. Resolve scene paths and hierarchy carefully; unsaved scene changes may be unavailable on disk.

### End Unity Codex Tasks Workflow
