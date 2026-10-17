// using System.Linq;
// using GuildMasterIsekai.Systems;
// using UnityEngine;
//
// namespace GuildMasterIsekai.TileGrid
// {
//     public sealed class BaseLayerRemoveTool : BuildTool
//     {
//         protected override bool Apply(GridData data, RectInt area, OperationCost payment, OperationArgs e, out string error)
//         {
//             error = null;
//             if (e.buildingMaterial == null)
//             {
//                 error = "Select a building material for floor costs and refunds.";
//                 return false;
//             }
//             var changed = false;
//             foreach (var p in area.allPositionsWithin)
//             {
//                 var cell = data[p.x, p.y].Clone();
//                 if (!cell.Floor) continue;
//                 for (var y = -2; y <= 2; y++)
//                 for (var x = -1; x <= 1; x++)
//                 {
//                     var at = p + new Vector2Int(x, y);
//                     if (data.Contains(at) && (data[at.x, at.y].Furniture ||
//                         data[at.x, at.y].slots.Any(t => t)))
//                     { error = "Remove furniture and toppings in the floor's 3 x 5 footprint first."; return false; }
//                 }
//                 payment.AddItem(e.buildingMaterial, cell.Floor.Cost);
//                 cell.Floor = null;
//                 data.ApplyState(cell);
//                 changed = true;
//             }
//             if (!changed) { error = "The brush contains nothing removable on this layer."; return false; }
//             return BaseLayerPlaceTool.Rebuild(data, out error) && GridEditing.Validate(data, out error);
//         }
//     }
// }
//