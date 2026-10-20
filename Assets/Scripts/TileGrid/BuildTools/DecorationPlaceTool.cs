// using System;
// using GuildMasterIsekai.Systems;
// using UnityEngine;
//
// namespace GuildMasterIsekai.TileGrid
// {
//     public sealed class ToppingLayerPlaceTool : BuildTool
//     {
//         protected override bool RequiresItem => true;
//         public override System.Collections.Generic.IEnumerable<object> GetCompatibleItems(IBuildBag bag) =>
//             System.Linq.Enumerable.Cast<object>(System.Linq.Enumerable.Select(bag.GetAvailableItem<ToppingTileSo>(), pair => pair.Item1));
//
//         protected override bool Apply(GridData data, RectInt area, OperationCost payment, OperationArgs e, out string error)
//         {
//             var item = e.item as ToppingTileSo;
//             error = null;
//             if (area.width != 1 || area.height != 1)
//             { error = "Topping placement requires a pixel brush."; return false; }
//             var position = area.min;
//             var slot = e.slot;
//             var cell = data[position.x, position.y].Clone();
//             if (!item || !item.Prefab || !cell.Furniture)
//             { error = "A topping prefab and supporting furniture are required."; return false; }
//             if (slot == -1) slot = Array.FindIndex(cell.slots, t => !t);
//             if (slot >= 0 && slot < cell.slots.Length && cell.slots[slot] == item) return true;
//             if (slot < 0 || slot >= cell.slots.Length || cell.slots[slot])
//             { error = "The furniture has no available requested topping slot."; return false; }
//             cell.slots[slot] = item;
//             payment.AddItem(item, -1);
//             data.ApplyState(cell);
//             return true;
//         }
//     }
// }
//