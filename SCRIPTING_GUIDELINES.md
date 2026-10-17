# Scripting guidelines

Applies to project-owned C# in `Assets/Script`. Based on the current scripts, root `.editorconfig`, and assembly definitions. External code in `Assets/Plugins` is outside this scope. Proposed convention changes are recorded in [SCRIPTING_SUGGESTIONS.md](SCRIPTING_SUGGESTIONS.md).

## Configuration and formatting

The root `.editorconfig` is the source of truth for configured formatting. It currently has no C# section, C# naming rules, or `root = true` declaration. Its general settings apply to C# unless an ancestor configuration supplies additional rules.

| Setting | Current value |
| --- | --- |
| Encoding | UTF-8 |
| Line endings | CRLF |
| Indentation | Four spaces; tab width four |
| Line length | 120 characters |
| Insert final newline | Disabled |
| Trim trailing whitespace | Disabled |
| JetBrains continuation indentation | Eight spaces |
| JetBrains formatter tags | `@formatter:off` / `@formatter:on`, enabled |
| Wrap on typing | Disabled in JetBrains settings |

Disabled insertion/trimming does not require deleting existing final newlines or adding trailing spaces. Treat 120 characters as the formatting target; it is not a compiler-enforced limit. JetBrains-specific settings may not be recognized by other editors.

Most space-indented scripts use block-scoped namespaces and opening braces on separate lines. Some files, especially under `Visitor`, use tabs and opening braces on the same line. Brace placement is currently an observed convention, not an explicit `.editorconfig` rule. For new files, follow the space-indented layout in `Model/Quest.cs`; avoid unrelated whole-file formatting when editing existing files.

Use blank lines between logical member groups and methods. Short getters, forwarding methods, and simple constructors commonly use expression bodies. Short guard clauses commonly omit braces; use a block when multiple statements belong together. Keep complex control flow readable rather than compressing it onto one line.

## Folder and namespace responsibilities

| Folder | Existing responsibility and namespace |
| --- | --- |
| `Model` | Serializable gameplay state and binary persistence; `GuildMasterIsekai.Model` |
| `ModelView` | `Drafts.DataView` bindings and save-data presentation; `GuildMasterIsekai.ModelView` |
| `Database` | Authored ScriptableObject entries and related enums; `GuildMasterIsekai.Database` |
| `System` | Shared services, database helpers, persistence, localization, and observable wrappers; `GuildMasterIsekai.System` |
| `Visitor` | Visitor spawning, behavior, animation, and timing; `GuildMasterIsekai.Visitor` |
| `Visitor/Spot` | Spot ownership and queues; `GuildMasterIsekai.Visitor.Spot` |
| `TileGrid` | Tile behavior; `GuildMasterIsekai.TileGrid` |
| `Editor` | Asset tools and custom inspectors; currently `Editor` |

`Generator.cs` sits at the script root in `GuildMasterIsekai`. The runtime assembly is `GuildMasterIsekai`; editor tools have a separate `GuildMasterIsekaiEditor` assembly definition. Keep new scripts near related behavior. The project `.csproj.DotSettings` files skip `Assets/Script` as a namespace folder, so namespaces do not include `Assets.Script`.

## Names and declarations

- Use PascalCase for types, methods, properties, enum members, and constants; prefix interfaces with `I`.
- Use camelCase for parameters, local variables, and serialized fields. Serialized fields commonly use `[SerializeField] private` and expose a PascalCase property when needed.
- Private caches and internal fields currently mix camelCase and `_camelCase`. Match the containing type until the project adopts a single rule; do not infer a universal underscore policy.
- Existing suffixes include `So` for database assets, `View` for data views, and `Utility` for helper classes. Preserve established public names when extending them.
- Give a new file the name of its main type. Existing exceptions include `DatabaseItem.cs` (`DatabaseEntrySo`), `GuildSpots.cs` (`HallSpots`), and the `VisitorAI.*.cs` behavior files; those files are separate classes, not partial declarations.
- Put `using` directives before the namespace. The common order is `System`, external libraries such as `Drafts`, project namespaces, then Unity namespaces; aliases appear afterward. Remove unused imports when practical.
- Target-typed `new()`, `var`, switch expressions, pattern matching, and `??=` already appear in the project. Use them when the type and behavior remain clear, within the project's supported C# version.

## State, assets, and presentation

Gameplay models commonly use `[Serializable]`, private serialized fields, and public getters. Database entries derive from `DatabaseEntrySo`, use `[CreateAssetMenu]`, and share IDs, localized text, and icons. Reuse those abstractions rather than introducing parallel representations for the same data.

Data views derive from `DataView<T>` and populate UI in `Subscribe()` through helpers such as `TrySetText`, `TrySetSprite`, and `TrySetData`. Check the base class contract before changing subscription behavior: existing overrides differ in whether they call `base.Subscribe()`. Keep gameplay decisions in the model or behavior code and presentation updates in views.

The code uses both `Drafts.Observable<T>` / `ObservableList<T>` and project `Watcher<T>`. Preserve the expected notification contract when modifying state. `Watcher<T>.Value` raises `OnChanged`; `SetSilently` deliberately bypasses it. Existing events commonly use the `OnChanged` / `OnReplied` naming pattern.

## Persistence and compatibility

Models implement `IBinarySave` with paired `Save(BinaryWriter)` and `Load(BinaryReader)` methods. Preserve identical field order and matching binary types. Existing code writes enums as integers, normalizes nullable strings before writing, and uses presence flags or save helpers for optional objects.

Initialize nested state before loading it. Preserve database references through the existing ID-based `WriteItem` / `ReadItem` helpers. A change to field order, enum values, or stored IDs can affect saved data; handle compatibility deliberately.

`SaveFileParser` separately stores JSON and invokes `ISaveData.BeforeSave` / `AfterLoad` when implemented. Do not assume the binary model layout and JSON output are interchangeable. Preserve serialized field names and Unity asset `.meta` files during refactors, or provide the appropriate migration.

## Lifetimes and editor tools

Pair event and UnityEvent subscriptions with cleanup at the matching lifetime boundary. `VisitorController` removes listeners in `OnDestroy`; visitor behaviors clear pending requests and call base disposal. Extend those cleanup paths when adding subscriptions, queue membership, spot ownership, or coroutines.

Visitor behavior belongs in `IVisitorAI` / `BaseVisitorAI` implementations, while the `VisitorAI` component owns activation and replacement. Keep disposal safe when called repeatedly, following the base class's disposed guard.

Keep `UnityEditor` dependencies out of player code through a correctly restricted editor assembly or conditional compilation. For asset-changing editor commands, follow `RuleTileOverrideFiller.cs`: validate selection, record Undo, mark changed assets dirty, and save the affected asset.

## Reviewing script changes

- Check formatting against `.editorconfig` and the conventions of the touched type.
- Check serialized references, saved-data layout, notification behavior, and cleanup when affected.
- Verify compilation in Unity for C# changes; exercise affected gameplay, view bindings, or editor commands as appropriate.
- Keep behavior changes and broad formatting or naming migrations in separate changes so each can be reviewed clearly.