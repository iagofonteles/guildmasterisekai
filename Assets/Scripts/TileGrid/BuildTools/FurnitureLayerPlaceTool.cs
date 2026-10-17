// using GuildMasterIsekai.Systems;
// using UnityEngine;
//
// namespace GuildMasterIsekai.TileGrid
// {
//     public sealed class FurnitureLayerPlaceTool : BuildTool
//     {
//         protected override bool RequiresItem => true;
//         public override System.Collections.Generic.IEnumerable<object> GetCompatibleItems(IBuildBag bag) =>
//             System.Linq.Enumerable.Cast<object>(System.Linq.Enumerable.Select(bag.GetAvailableItem<FurnitureTileSo>(), pair => pair.Item1));
//
//         protected override bool Apply(GridData data, RectInt area, OperationCost payment, OperationArgs e, out string error)
//         {
//             var item = e.item as FurnitureTileSo;
//             error = null;
//             if (!item || !item.Tile || (item.ToppingSlots?.Count ?? 0) > 1024)
//             { error = "Select furniture with a tile and at most 1024 topping slots."; return false; }
//             foreach (var p in area.allPositionsWithin)
//             {
//                 var cell = data[p.x, p.y].Clone();
//                 if (cell.Furniture == item) continue;
//                 if (cell.Furniture) { error = "Furniture destination is occupied."; return false; }
//                 cell.Furniture = item;
//                 cell.slots = new ToppingTileSo[item.ToppingSlots?.Count ?? 0];
//                 payment.AddItem(item, -1);
//                 data.ApplyState(cell);
//             }
//             return GridEditing.Validate(data, out error);
//         }
//     }
// }
//