// using System;
// using GuildMasterIsekai.Systems;
// using UnityEngine;
//
// namespace GuildMasterIsekai.TileGrid
// {
//     public sealed class FurnitureLayerRemoveTool : BuildTool
//     {
//         protected override bool Apply(GridData data, RectInt area, OperationCost payment, OperationArgs e, out string error)
//         {
//             error = null;
//             var changed = false;
//             foreach (var p in area.allPositionsWithin)
//             {
//                 var cell = data[p.x, p.y].Clone();
//                 if (!cell.Furniture) continue;
//                 payment.AddItem(cell.Furniture, 1);
//                 foreach (var topping in cell.slots)
//                     if (topping) payment.AddItem(topping, 1);
//                 cell.Furniture = null;
//                 cell.slots = Array.Empty<ToppingTileSo>();
//                 data.ApplyState(cell);
//                 changed = true;
//             }
//             if (!changed) { error = "The brush contains nothing removable on this layer."; return false; }
//             return GridEditing.Validate(data, out error);
//         }
//     }
// }
//