# Codex Tasks

Open **Tools > Codex Tasks** in Unity. Install Codex CLI and run `codex login` in a terminal first. If Unity cannot find `codex.exe` through PATH, set its absolute executable path in the window. Restart Unity after changing PATH.

Press **Start Codex** to initialize the managed project-root AGENTS.md section, ignore temporary reports at the repository root, and validate CLI features/authentication. Idle is a logical session; each submission launches its own non-interactive `codex exec` process from the Unity project root.

**Answer Question** uses `--sandbox read-only`; the integration saves stdout as `Reports/QuickAnswer.txt`. **Run Task** uses `--sandbox workspace-write`, sends the exact unique report path, and saves the final response itself. Reports use the local date and an increasing daily sequence. All report output stays inside this plugin folder, overriding the original prompt's `Assets/Codex Tasks/` location. Root AGENTS.md and repository .gitignore are the required workflow configuration exceptions.

Add reference fields and drag scene objects or assets onto them. Scene context includes hierarchy sibling indices, scene path, global ID, and unsaved-change warnings. Question and object references serialize with the window across domain reloads; initialization is deliberately revalidated after a reload or reopening.

Stop/Cancel terminates the Windows process tree. Closing the window, recompiling, or exiting also requests cancellation. Interrupted implementation tasks may leave partial changes. No commits are created. The tool does not bypass approval/sandbox policies: operations requiring interactive approval may fail and must be handled outside this window. Configured MCP tools and external integrations retain their own permissions; the CLI filesystem sandbox governs shell/file access.

Task file lists are agent-reported and explicitly unverified. Review them and existing user modifications before accepting changes. Failed/cancelled tasks keep a failure marker at the reserved report path; errors and stderr appear in the window. Existing tracked reports cannot be untracked by an ignore rule.

Validation: C# compiled against this project's Unity 6000.5.9f1 reference assemblies. Standalone workflow checks cover preserving original AGENTS content, repeated updates, malformed sections, Windows title sanitation, and unique incrementing reports. CLI exec/help flags were inspected. Authenticated model requests and interactive Unity rendering, drag/drop, cancellation/reload, and asset imports still require Editor validation. The available command environment could not resolve a home directory for `codex login status`.

CLI behavior reference: [official OpenAI non-interactive documentation](https://learn.chatgpt.com/docs/non-interactive-mode).
