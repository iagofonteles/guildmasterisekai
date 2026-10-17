# Building controls

`Guild Hall Montagem` has `GridBuilderInput` and `GridBuilderUI` attached to its Grid.
The input uses that scene's camera explicitly; it does not require a MainCamera tag.
Enter Play mode, select a layer, a tool and a tile, then click the grid.

- Pixel: click or hold and drag to edit each visited cell. Each cell is one undo step.
- Rectangle: drag and release to apply the full rectangle as one undo step.
- Paint: available on the base layer. Remove: available on all layers.
- Furniture move with Pixel: drag from the source cell to the destination.
- Furniture move with Rectangle: drag the source rectangle, then click its new
  bottom-left cell. Right-click cancels pending gestures and selections.
- Topping placement always uses single cells and the first available slot.
- Undo/Redo buttons use the builder's existing command and inventory checks.

Tile choices come from `GridBuilder.GetAvailable<T>()`, using its configured build bag.
The scene uses `InfiniteBuildBag`, which reads definitions from `Game.Database`.
Selection buttons use the asset name and optional database icon; base tiles also show cost.

For another scene, add `GridBuilderInput` and `GridBuilderUI` to the builder object and
assign its Camera and UI reference on the input. A tagged MainCamera is the fallback.
The UI creates a UIDocument and a runtime PanelSettings when none are assigned, and loads
the layout, custom theme and styles from `UI/Resources/TileGrid`. An existing PanelSettings
can be assigned instead. The runtime-owned PanelSettings is destroyed with the UI.

The full-screen UI root ignores picking. Only the toolbar and controls intercept input.
Before polling grid edits, the input synchronously picks active UIDocument panels in
screen coordinates and checks pointer capture. The document list is cached and refreshed
on each left press, including inactive documents so opening existing UI is recognized.
A press starting on UI remains owned by UI until release; entering UI cancels a pending
grid drag. Pixel edits already committed before entering UI remain committed.

The controller polls `UnityEngine.InputSystem.Mouse.current` in Update; no InputAction
asset is required. Screen rays intersect the grid's transformed cell plane, including
its saved Z plane. Missing mouse/camera, focus loss and disabling the component cancel
pending gestures. Other gameplay mouse controllers must implement their own UI blocking.

Validation: the Editor validation suite includes deterministic pointer-gesture checks
for UI ownership, rectangle cancellation/commit, pixel placement, furniture movement,
undo and unsupported tool selection. These checks compile but have not been executed
in Unity for this change because the Editor connection was unavailable. UXML/USS import
and actual mouse interaction also require an Editor/Play-mode check.

## Implementation report

Summary: Added mouse-driven building and a UI Toolkit selection document, wired into the
existing building test scene. All new scripts and UI resources are under TileGrid/UI.

Completed work: Layer/tool/brush and available-tile selection; pixel and rectangle editing;
pixel/group furniture movement; topping placement/removal; undo/redo and status feedback;
synchronous UI picking/capture checks and gesture ownership; transformed-grid ray projection.
GridBuilder exposes available definitions and origin without requiring full snapshots.

Validation results: dotnet build GuildMasterIsekai.csproj --no-restore --verbosity quiet
passed with 0 errors and 4 existing CS0067 warnings. Added two pointer-input validation
scenarios to the existing Editor suite; they compile but were not executed. The Editor
connection attempt returned Unity not detected (no fresh discovery files found).

Limitations: Scene wiring was applied to the saved scene on disk; unsaved Editor state
was unavailable. UXML/USS import, actual UI picking and physical mouse interaction need
Play-mode validation. The runtime PanelSettings is owned by the UI unless one is supplied.
Other gameplay controllers are not modified. No commit was created.

Modified assets / files:
- Assets/Scripts/TileGrid/UI/GridBuilderInput.cs
- Assets/Scripts/TileGrid/UI/GridBuilderInput.cs.meta
- Assets/Scripts/TileGrid/UI/GridBuilderUI.cs
- Assets/Scripts/TileGrid/UI/GridBuilderUI.cs.meta
- Assets/Scripts/TileGrid/UI/Resources.meta
- Assets/Scripts/TileGrid/UI/Resources/TileGrid.meta
- Assets/Scripts/TileGrid/UI/Resources/TileGrid/GridBuilder.uxml
- Assets/Scripts/TileGrid/UI/Resources/TileGrid/GridBuilder.uxml.meta
- Assets/Scripts/TileGrid/UI/Resources/TileGrid/GridBuilder.uss
- Assets/Scripts/TileGrid/UI/Resources/TileGrid/GridBuilder.uss.meta
- Assets/Scripts/TileGrid/UI/Resources/TileGrid/GridBuilderTheme.tss
- Assets/Scripts/TileGrid/UI/Resources/TileGrid/GridBuilderTheme.tss.meta
- Assets/Scripts/TileGrid/UI/README.md
- Assets/Scripts/TileGrid/UI/README.md.meta
- Assets/Scripts/TileGrid/GridBuilder.cs
- Assets/Scripts/TileGrid/Editor/GridBuildingValidation.cs
- Assets/Scenes/Guild Hall Montagem.unity

