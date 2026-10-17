# TileGrid runtime building

Add `GridBuilder` to a scene object and assign an existing Tilemap as **Initial Tilemap**.
Its current `cellBounds` define the playable rectangle, including the bounds origin.
Use a single Z plane. Compress the source bounds in the Editor first if its stored bounds
include unwanted empty margins. Initialization snapshots every tile, including null cells,
hides the source renderer, and creates runtime Base, Furniture and Toppings objects.
The source Tilemap's tile data remains unchanged. Destroying the builder restores its renderer.

Assign an `InfiniteBuildBag` component to **Build Bag** for testing. It lists definitions
using `Game.Database.GetAll<T>()` and accepts all payments. For a real inventory, initialize
from a controller before `Start`:

```csharp
var bag = new InventoryBuildBag(inventory, currency);
builder.Initialize(setupTilemap, bag);
```

`inventory` is a caller-owned `Drafts.Inventory.ListInventory<DatabaseEntrySo>`.
Furniture and toppings consume their own definition objects. `bag.Currency` is the
associated currency balance; persist it and the inventory through the owning game system.
To change currency externally, use a `BuildPayment` with `AddCurrency`, then `TryApply`.
The adapter checks all balances and stack capacities before applying a transaction.
Drafts inventory notifications are synchronous and per-item: their subscribers must not
throw, mutate the inventory, change stack limits, or attempt another build payment during
a transaction. Use a custom `IBuildBag` for inventory systems requiring batch notifications.

## Tool API

Input handling and UI remain in the caller. Convert pointers with `WorldToCell`, then
call one of the following. Each successful call produces one Undo/Redo command.
`Changed` notifies the caller after a successful edit; failures return `false` and set
`LastError`. No input bindings, scene edits, or inventory objects are created automatically.

```csharp
var pixel = GridBuilder.Pixel(cell);
var rectangle = GridBuilder.Rectangle(dragStart, dragEnd); // inclusive endpoints
builder.TryPlaceFloor(rectangle, guildTile);
builder.TryPaint(rectangle, guildTile);
builder.TryPlaceFurniture(pixel, guildObject);
builder.TryPlaceTopping(cell, guildTopping); // first free slot, or provide slot index
builder.TryRemove(rectangle, BuildLayer.Base);
builder.TryRemove(pixel, BuildLayer.Topping, pointerWorldPosition);
builder.TryMove(rectangle, cellOffset);
builder.Undo();
builder.Redo();
```

- Brushes reject out-of-bounds rectangles. Each call is all-or-nothing. Drag controllers
  may make one call at release for a single rectangle command; individual pixel calls
  produce individual commands. Revisited floor anchors are skipped without another charge.
- Furniture occupies one cell. Rectangle placement fills every selected cell; moving
  translates all selected furniture and toppings as a group. Destinations must match
  `BuildOn` and be unoccupied after the group vacates its original cells. Moves are free.
- Topping placement uses the requested free slot or the first free slot. Slots are local
  transform-space XY offsets from the cell center. Removal chooses one topping per cell,
  nearest in world space to the pointer, or lowest occupied slot index without a pointer.
  Equal distances choose the lowest slot index. Prefab rendering settings control appearance.
- Placed floor anchors generate the exact 3 x 5 pattern in `GridBuilding.txt`. Pattern
  overlap uses Floor > Wall > Roof precedence. Existing tiles of the required type are
  preserved regardless of material. New materials use the first matching anchor in
  row-major coordinate order. Existing floor anchors are never replaced by generated tiles.
- Painting changes only brush cells, preserves each tile's Floor/Wall/Roof type, costs
  nothing, and cannot paint cells displaying their initial terrain. Painting a floor
  changes its material definition for subsequent reconstruction.
- Only paid floor anchors can be removed/refunded. Furniture or toppings anywhere in
  their 3 x 5 footprint block removal. Cleared cells restore their own initial terrain;
  cells needed by surviving anchors remain generated. Initial terrain cannot be refunded.
- Placement and removal use the floor definition's Cost.
  History retains the last 64 commands by default. Undo/Redo applies the recorded inverse
  or forward inventory changes. If currency, items, or refund capacity are unavailable,
  grid and history remain unchanged. A new successful edit clears Redo.

## Saving

`GridBuilder` and `GridData` implement `Drafts.SaveData.IBinarySave`.
Call `Save(BinaryWriter)` / `Load(BinaryReader)` through the owning save system,
or `SaveFile(path)` / `LoadFile(path)` with an explicit caller-selected path.
Save the inventory and currency alongside the grid; grid loading does not spend or refund
items. Initialize the builder before loading. Successful loading clears history.

Tiles resolve through `Game.Database`; no tile catalog is required. `DbAutoTile` assets
must link to their registered `BaseTileSo` definition and match its Floor/Wall/Roof variant.
`DbTile` assets must link to their registered `FurnitureTileSo` definition and match its Tile.
Definitions require nonzero database IDs. Plain tiles and missing or inconsistent links
are rejected when saving. Existing catalog-specific terrain keys cannot be loaded;
existing database variant keys retain their format.

The versioned save includes dimensions, origin, every cell's initial tile, painted tile,
floor anchor, furniture, and topping slot assignments. It stores stable
database references, never Unity instance IDs. Defaults are saved independently
of the current setup Tilemap, so removal after loading restores the saved terrain even
if setup tiles changed. Missing references, malformed dimensions, invalid slots, truncated
data, unsupported versions, and inconsistent floor layouts are rejected before replacing
live state. `SaveFile` stages bytes and replaces an existing file only after serialization.
The path's parent directory must already exist.

## Validation

Run **Game / TileGrid / Run Validation** outside Play mode, or call
`GridBuildingValidation.Run()` from an Editor integration. Checks use an isolated preview
scene, temporary in-memory assets and a temporary database reference restored afterward.
They do not save scene or asset changes. See `ImplementationReport.md` for recorded results.

## Brushes and tools

`IBuildBrush.GetArea(first, last)` converts cell endpoints to an area. `BuildPixelBrush`
selects the current (last) cell; `BuildRectBrush` includes both endpoints in either direction.
`GridBuilder.Pixel` and `Rectangle` use these implementations.

`IBuildTool` exposes its selected database definition and prepares an `IBuildCommand` using
`CreateCommand(data, area, bag, out command, out error)`. Preparation reads a sparse overlay
and copies only cells requested for editing; it does not mutate live state or inventory.
Use `builder.TryUseTool(area, tool)` to retain inventory checks, atomic commits, rendering,
and undo/redo. Existing TryPlace/TryPaint/TryRemove/TryMove methods dispatch to the layer tools.
Base placement and painting take a floor definition;
furniture and topping placement take their corresponding definitions. Topping placement
also accepts a slot index and requires a single cell. Topping removal takes a cell-to-slot
resolver; the builder's removal wrapper supplies nearest-slot selection from a world pointer.
`FurnitureLayerMoveTool(delta)` moves the selected furniture and attached toppings together,
with no inventory cost. Shared grid bounds checks, floor reconstruction and state validation
remain in `GridEditing`.

## Command history and rendering

All eight layer tools inherit `BuildTool` and produce a shared recorded-cell-change command.
`Do` applies recorded after states and payment; `Undo` restores recorded before states and
inverse payment. Redo calls `Do` again without recalculating prices, selections or nearest
slots. Failed payments do not change cells, history or the modified set. Commands copy
mutable topping arrays and retain only changed cells; edits and furniture moves no longer
clone the entire grid. `Snapshot()` remains an explicit full clone. Save/load validation
also retains a full clone for checking reconstructed floor ownership.

Committed changes preserve live cell identity and enter `GridData.Modified`, a
`HashSet<GridCell>`. Each cell has its owning `Grid` and coordinate `Position`; clones and
loaded cells are rebound. Internal tool writes request `EditCell` on the staging grid;
commits pass through `ApplyState`. External callers should use the builder/tool APIs.

Rendering writes changed base/furniture tiles with individual `Tilemap.SetTile` calls,
including nulls to erase removed tiles, and refreshes the surrounding 3 x 3 neighborhood.
Topping instances are cached by cell and slot; unrelated instances are retained. Successful
rendering clears the processed dirty cells. Failed rendering retains dirty cells and pending
neighbor refreshes for retry on a later render. Loading clears old rendered bounds and
topping instances, then renders every loaded cell. Undo history and rendering caches are
transient and do not change save version 2.

Floor ownership reconstruction still scans the grid and allocates ownership/type arrays.
This change removes full-grid copies from edits and full-map redraws during ordinary edits;
it does not eliminate all work proportional to grid dimensions.
