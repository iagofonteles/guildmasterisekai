// using GuildMasterIsekai.Systems;
// using UnityEngine;
//
// namespace GuildMasterIsekai.TileGrid
// {
//     public sealed class BaseLayerPaintTool : BuildTool
//     {
//         public override bool RequiresItem => true;
//         public override System.Collections.Generic.IEnumerable<object> GetCompatibleItems(IBuildBag bag) =>
//             bag.GetUnlockedTiles();
//
//         protected override bool Apply(GridData data, RectInt area, OperationCost payment, OperationArgs e, out string error)
//         {
//             var tile = e.item as BaseTileSo;
//             error = null;
//             if (!BaseLayerPlaceTool.HasVariants(tile)) { error = "Select a GuildTile with valid floor, wall and roof variants."; return false; }
//             var changed = false;
//             foreach (var p in area.allPositionsWithin)
//             {
//                 var cell = data[p.x, p.y].Clone();
//                 if (cell.BaseTile == cell.DefaultTile ||
//                     cell.BaseTile is not DbAutoTile current) continue;
//                 var replacement = GridData.Variant(tile, current.Type);
//                 if (!replacement || replacement == cell.BaseTile) continue;
//                 cell.BaseTile = replacement;
//                 // Painting an anchor changes its material for future neighbor reconstruction,
//                 // but does not repaint its existing surrounding cells.
//                 if (cell.Floor) cell.Floor = tile;
//                 data.ApplyState(cell);
//                 changed = true;
//             }
//             if (!changed) { error = "The brush contains no paintable changes."; return false; }
//             return GridEditing.Validate(data, out error);
//         }
//     }
// }
//