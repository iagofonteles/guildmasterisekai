// using System;
// using GuildMasterIsekai.Systems;
// using UnityEngine;
//
// namespace GuildMasterIsekai.TileGrid
// {
//     public sealed class ToppingLayerRemoveTool : BuildTool
//     {
//         protected override bool Apply(GridData data, RectInt area, OperationCost payment, OperationArgs e, out string error)
//         {
//             error = null;
//             var changed = false;
//             foreach (var p in area.allPositionsWithin)
//             {
//                 var cell = data[p.x, p.y].Clone();
//                 var slot = e.nearestSlot != null ? e.nearestSlot(p) : Array.FindIndex(cell.slots, t => t);
//                 if (slot < 0 || slot >= cell.slots.Length || !cell.slots[slot]) continue;
//                 payment.AddItem(cell.slots[slot], 1);
//                 cell.slots[slot] = null;
//                 data.ApplyState(cell);
//                 changed = true;
//             }
//             if (!changed) { error = "The brush contains nothing removable on this layer."; return false; }
//             return GridEditing.Validate(data, out error);
//         }
//     }
// }
//