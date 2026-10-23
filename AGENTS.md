# Task-file workflow

- Do not guard-check against null on invalid state unless the variable is intended to be null at that point. Exeptions are Awake, Start, Constructor and Initialization, here you should check for valid state 

### Unity Codex Tasks Workflow

- This is a Unity project. Respect its Unity version, assembly definitions, namespaces, project structure, and existing coding style.
- When the user says "execute {filename}" read the txt file and follow its current instructions. Do not reuse the contents from a previous turn; the user may have edited the file.
- When the user adds "codeonly" or similar to a request, make it a fast iteration. only edit text files, dont run builds or tests, dont try to validate outsite code context.
- After execunting task from the task file, append to the task file a list of modified files and a brief report the task with the found problems during execution
- Prompts starting with "question" or similar are read-only questions. Inspect code and referenced objects, but do not modify, create, rename, or delete project files. Return plain text.
- Reports must contain a summary, completed work, validation results, limitations, and a final section titled 'Modified assets / files:' listing every known added, modified, or deleted project-relative path. State clearly when the list cannot be verified. Never invent results.
- Referenced Unity objects are contextual targets, not edit permission. Resolve scene paths and hierarchy carefully; unsaved scene changes may be unavailable on disk.

### End Unity Codex Tasks Workflow

### Ponytail project defaults

- Use the installed `ponytail:ponytail` skill in **full** mode for coding tasks unless the user chooses another level or says "stop ponytail" or "normal mode". The user's choice applies for the rest of that session.
- Read the affected code and its callers before editing. Make the smallest complete change, reuse existing project patterns, and avoid new dependencies or abstractions unless the task requires them.
- Preserve Unity asset GUIDs and `.meta` files, serialized field compatibility, assembly boundaries, and existing scene/prefab references. Do not change unrelated assets.
- Validate in proportion to the change. For non-trivial logic, add or use a focused check; respect `codeonly` by editing text only and reporting that builds and tests were skipped.
- Keep reports concise while including the required Unity workflow report fields. Put skipped checks and known risks immediately before the final `Modified assets / files:` section.

Usage in Codex (requires the Ponytail plugin installed in that environment):

- `$ponytail:ponytail lite`, `$ponytail:ponytail full`, or `$ponytail:ponytail ultra`: choose a level. Full favors the smallest complete solution; lite names a smaller option; ultra also challenges unnecessary scope.
- `$ponytail:ponytail-review`: review a change.
- `$ponytail:ponytail-audit`: audit the whole repository without editing it.
- `$ponytail:ponytail-debt`: list `shortcut:` comments and older `ponytail:` comments.
- `$ponytail:ponytail-help`: show the plugin's usage reference.
- Say "stop ponytail" or "normal mode" to disable it for the session. Project defaults apply again in a new session.
