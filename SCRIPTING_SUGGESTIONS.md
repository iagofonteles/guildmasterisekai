# Suggestions for the current scripting format

These are proposals based on `Assets/Script` and `.editorconfig`, not rules already enforced by the project. No scripts or configuration were changed as part of this review. The current baseline is documented in [SCRIPTING_GUIDELINES.md](SCRIPTING_GUIDELINES.md).

## 1. Make C# formatting explicit

The configuration contains extensive settings for other languages but no C# section. Consequently, four-space indentation is configured, while brace placement, imports, naming, and expression-body preferences depend on editor defaults and existing files.

Add a focused `[*.cs]` section after agreeing on the desired style. A reasonable starting point matching `Model/Quest.cs` is:

```editorconfig
[*.cs]
csharp_new_line_before_open_brace = all
csharp_indent_case_contents = true
csharp_indent_switch_labels = true
csharp_space_after_keywords_in_control_flow_statements = true
csharp_space_around_binary_operators = before_and_after
```

These entries are a proposed baseline, not a complete formatter setup. Verify their effect in the team's editors before applying a broad cleanup. Preserve compact expression bodies where they improve readability.

## 2. Resolve indentation and brace drift gradually

`Visitor/VisitorAI.cs`, `Model/GuildHall.cs`, and `ModelView/StatsView.cs` contain tab-indented or same-line-brace layouts, while `Model/Quest.cs` and `Visitor/VisitorAI.Base.cs` mostly use spaces and separate-line braces. Some files mix both layouts internally.

Use four spaces for new indentation immediately, since that is already configured. Once brace rules are agreed, format one folder at a time in dedicated changes. Avoid combining mechanical reformatting with gameplay fixes.

## 3. Simplify configuration scope and whitespace policy

Consider adding `root = true` to make formatting independent of parent-directory settings. Review whether the exported CSS, Python, GDScript, and other language sections are useful to this repository before removing any of them.

Consider enabling `insert_final_newline = true` and `trim_trailing_whitespace = true` for C# specifically. This would reduce incidental whitespace changes, but it changes the current policy and should be adopted deliberately. Keep CRLF unless the team chooses a repository-wide alternative. A Markdown override can preserve intentional trailing spaces if whitespace trimming is later enabled globally.

## 4. Choose one private-field naming rule

`Game` uses `_database`, `FreelancerVisitorAI` uses `_guild` and `_hall` alongside `pendingRequest`, and `BaseVisitorAI` uses names such as `runner` and `leaveQueue`. Serialized fields predominantly use plain camelCase.

Keep serialized fields camelCase and choose either camelCase or `_camelCase` for other private fields. Encode the agreed rule with naming settings supported by the team's editors. Start with new code; renaming serialized fields requires preserving asset and save compatibility.

## 5. Align filenames and domain vocabulary

Consider renaming `System/DatabaseItem.cs` to `DatabaseEntrySo.cs` and `Visitor/Spot/GuildSpots.cs` to `HallSpots.cs`. The files `VisitorAI.Base.cs`, `VisitorAI.Costumer.cs`, and `VisitorAI.Freelancer.cs` declare independent classes; matching their type names would make navigation clearer.

Confirm whether `Costumer` means customer in this domain. If so, plan a consistent `Customer` rename across code, assets, and persistence rather than correcting isolated occurrences. Also clarify whether `Database/StatSo.cs`'s `Resistance` and `Model/Stats.cs`'s `Vigor` represent the same stat. Preserve existing enum numeric meanings during any migration.

Move or rename Unity scripts together with their `.meta` files. Treat public API, serialized-field, and serialized-type changes as compatibility work, not formatting cleanup.

## 6. Clarify editor boundaries

`Editor/GuildMasterIsekaiEditor.asmdef` currently has an empty `includePlatforms` list. Consider restricting it to `Editor`; the folder name alone should not be relied on to restrict an explicitly defined assembly. `RuleTileOverrideFiller.cs` imports `UnityEditor` without a conditional guard, so verify a player build after adjusting the boundary.

Consider using `GuildMasterIsekai.Editor` instead of the generic `Editor` namespace, and give the editor assembly an appropriate root namespace. Keep this migration separate from formatter changes.

## 7. Agree on a small member-layout convention

Constructor and property placement varies between model classes. A practical default is fields, events, constructors, properties, public methods, then implementation helpers. Keep paired `Save` and `Load` methods adjacent. Allow short companion interfaces or enums in a file when they are tightly coupled, as in `Watcher.cs` and `Stats.cs`.

Document when single-line guards may omit braces. Expand multi-statement one-line blocks, such as the early return in `FreelancerVisitorAI.WaitReply`, to make later edits easier to review. Use comments to explain constraints, and replace abandoned commented code with tracked work where appropriate.

## 8. Record shared behavior contracts

`Watcher<T>` and `Drafts.Observable<T>` coexist, `IDisplay.OnChanged` is declared by multiple models without visible invocations, and view overrides differ in whether they call `base.Subscribe()`. Document notification and subscription expectations after checking the external base implementations. Avoid prescribing a blanket base call or wrapper replacement without that check.

Keep functional follow-up separate from formatting: `Problem.cs` already identifies unfinished outcome persistence with TODOs, and `Assignment.GetTask()` contains unimplemented branches. Give these explicit tracked work items rather than allowing a style pass to imply that the behavior is complete.

## Suggested adoption order

1. Agree on the C# formatter settings and field naming policy.
2. Update `.editorconfig` and verify representative scripts in the team's editors.
3. Check the editor assembly restriction with Unity compilation and a player build.
4. Apply formatting in small dedicated changes, then handle naming migrations with compatibility checks.
5. Add an automated formatting check only once the chosen formatter produces stable results for the agreed style.