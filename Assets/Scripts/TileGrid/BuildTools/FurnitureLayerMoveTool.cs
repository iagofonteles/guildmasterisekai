// using System;
// using System.Collections.Generic;
// using GuildMasterIsekai.Systems;
// using UnityEngine;
//
// namespace GuildMasterIsekai.TileGrid
// {
//     /// <summary>Moves selected furniture and its toppings together without inventory cost.</summary>
//     public sealed class FurnitureLayerMoveTool : BuildTool
//     {
//         protected override bool Apply(GridData data, RectInt area, OperationCost payment, OperationArgs e, out string error)
//         {
//             var delta = e.delta;
//             error = null;
//
//             if (delta == Vector2Int.zero)
//             {
//                 error = "Move requires a different destination.";
//                 return false;
//             }
//
//             var before = new Dictionary<Vector2Int, GridCell>();
//             var positions = new List<Vector2Int>();
//             foreach (var p in area.allPositionsWithin)
//                 if (data[p.x, p.y].Furniture)
//                 {
//                     positions.Add(p);
//                     before.Add(p, data[p.x, p.y].Clone());
//                 }
//
//             if (positions.Count == 0)
//             {
//                 error = "The selection contains no furniture.";
//                 return false;
//             }
//
//             foreach (var p in positions)
//             {
//                 var cell = data[p.x, p.y].Clone();
//                 cell.Furniture = null;
//                 cell.slots = Array.Empty<ToppingTileSo>();
//                 data.ApplyState(cell);
//             }
//
//             foreach (var p in positions)
//             {
//                 var target = p + delta;
//
//                 if (!data.Contains(target) || data[target.x, target.y].Furniture)
//                 {
//                     error = "The move destination is outside the grid or occupied.";
//                     return false;
//                 }
//
//                 var cell = data[target.x, target.y].Clone();
//                 cell.Furniture = before[p].Furniture;
//                 cell.slots = (ToppingTileSo[])before[p].slots.Clone();
//                 data.ApplyState(cell);
//             }
//
//             return GridEditing.Validate(data, out error);
//         }
//     }
// }
//