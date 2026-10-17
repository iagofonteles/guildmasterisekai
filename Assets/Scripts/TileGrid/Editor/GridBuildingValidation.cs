// #if UNITY_EDITOR
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Reflection;
// using Drafts.Inventory;
// using GuildMasterIsekai.Systems;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.Tilemaps;
// using Object = UnityEngine.Object;
//
// namespace GuildMasterIsekai.TileGrid
// {
//     /// <summary>Isolated Editor checks: no scene, asset, inventory or database changes are saved.</summary>
//     public static class GridBuildingValidation
//     {
//         private static int assertions;
//
//         [MenuItem("Game/TileGrid/Run Validation")]
//         public static void RunMenu() => Debug.Log(Run());
//
//         public static string Run()
//         {
//             if (EditorApplication.isPlayingOrWillChangePlaymode)
//                 throw new InvalidOperationException("Run TileGrid validation outside Play mode.");
//             assertions = 0;
//             var completed = new List<string>();
//             Case("mouse gestures respect UI ownership and rectangle cancellation", f =>
//             {
//                 var cameraObject = new GameObject("Input validation camera", typeof(Camera));
//                 cameraObject.transform.SetParent(f.Source.transform.parent, false);
//                 var camera = cameraObject.GetComponent<Camera>();
//                 camera.enabled = false;
//                 camera.orthographic = true;
//                 camera.orthographicSize = 8;
//                 camera.pixelRect = new Rect(0, 0, 800, 600);
//                 camera.transform.position = new Vector3(0, 0, -10);
//                 var input = f.Builder.gameObject.AddComponent<GridBuilderInput>();
//                 Fixture.Set(input, "builder", f.Builder);
//                 Fixture.Set(input, "viewCamera", camera);
//                 input.SetLayer(BuildLayer.Furniture);
//                 input.SelectTile(f.Chair);
//                 input.SetBrush(GridBuildBrush.Rectangle);
//                 Vector2 ScreenAt(Vector3Int cell) => camera.WorldToScreenPoint(f.Builder.BaseMap.GetCellCenterWorld(cell));
//                 var first = ScreenAt(Vector3Int.zero);
//                 var last = ScreenAt(Vector3Int.right);
//                 input.ProcessPointer(first, true, true, false, false, true);
//                 input.ProcessPointer(last, false, true, false, false, false);
//                 input.ProcessPointer(last, false, false, true, false, false);
//                 Check(!f.Builder.CanUndo && f.Inventory.Count(f.Chair) == 5, "UI-started press cannot leak into world after leaving UI");
//                 input.ProcessPointer(first, true, true, false, false, false);
//                 input.ProcessPointer(last, false, true, false, false, true);
//                 input.ProcessPointer(last, false, false, true, false, false);
//                 Check(!f.Builder.CanUndo && f.Inventory.Count(f.Chair) == 5, "Entering UI cancels a pending rectangle");
//                 input.ProcessPointer(first, true, true, false, false, false);
//                 Check(!f.Builder.CanUndo, "Rectangle does not commit before release");
//                 input.ProcessPointer(last, false, false, true, false, false);
//                 Check(f.LiveData[0, 0].Furniture == f.Chair && f.LiveData[1, 0].Furniture == f.Chair, "Released rectangle uses both endpoints");
//                 Check(f.Inventory.Count(f.Chair) == 3 && f.Builder.CanUndo, "Rectangle input records one atomic edit");
//                 input.Undo();
//                 Check(f.Inventory.Count(f.Chair) == 5 && !f.LiveData[0, 0].Furniture && !f.LiveData[1, 0].Furniture, "UI undo restores the complete rectangle");
//             }, completed);
//             Case("pixel mouse placement and move dispatch", f =>
//             {
//                 var cameraObject = new GameObject("Pixel input camera", typeof(Camera));
//                 cameraObject.transform.SetParent(f.Source.transform.parent, false);
//                 var camera = cameraObject.GetComponent<Camera>();
//                 camera.enabled = false;
//                 camera.orthographic = true;
//                 camera.orthographicSize = 8;
//                 camera.pixelRect = new Rect(0, 0, 800, 600);
//                 camera.transform.position = new Vector3(0, 0, -10);
//                 var input = f.Builder.gameObject.AddComponent<GridBuilderInput>();
//                 Fixture.Set(input, "builder", f.Builder);
//                 Fixture.Set(input, "viewCamera", camera);
//                 input.SetLayer(BuildLayer.Furniture);
//                 input.SelectTile(f.Chair);
//                 Vector2 ScreenAt(Vector3Int cell) => camera.WorldToScreenPoint(f.Builder.BaseMap.GetCellCenterWorld(cell));
//                 var first = ScreenAt(Vector3Int.zero);
//                 var last = ScreenAt(Vector3Int.right);
//                 input.ProcessPointer(first, true, true, false, false, false);
//                 input.ProcessPointer(first, false, false, true, false, false);
//                 Check(f.LiveData[0, 0].Furniture == f.Chair && f.Inventory.Count(f.Chair) == 4, "Pixel press places once");
//                 input.SetAction(GridBuildAction.Move);
//                 input.ProcessPointer(first, true, true, false, false, false);
//                 input.ProcessPointer(last, false, false, true, false, false);
//                 Check(!f.LiveData[0, 0].Furniture && f.LiveData[1, 0].Furniture == f.Chair, "Pixel move drags source to destination");
//                 Check(f.Inventory.Count(f.Chair) == 4, "Mouse move is free");
//                 input.SetLayer(BuildLayer.Topping);
//                 Check(input.Action == GridBuildAction.Place, "Switching layers resets an unsupported tool");
//                 input.SetAction(GridBuildAction.Paint);
//                 Check(input.Action == GridBuildAction.Place, "Paint is unavailable on topping layer");
//             }, completed);
//             Case("changed tile refreshes adjacent rendering", f =>
//             {
//                 var probe = ScriptableObject.CreateInstance<RefreshProbeTile>();
//                 try
//                 {
//                     f.Builder.FurnitureMap.SetTile(Vector3Int.right, probe);
//                     probe.Reads = 0;
//                     Check(f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.zero), f.Chair), f.Builder.LastError);
//                     Check(probe.Reads > 0, "A changed furniture tile refreshes its adjacent tile data");
//                 }
//                 finally { Object.DestroyImmediate(probe); }
//             }, completed);
//             Case("sparse command preparation, replay and dirty cells", f =>
//             {
//                 var data = f.Builder.Snapshot();
//                 var original = data.Clone();
//                 var anchor = data[0, 0];
//                 Check(anchor.Grid == data && anchor.Position == Vector2Int.zero, "Snapshot cell ownership/position rebound");
//                 var tool = new BaseLayerPlaceTool(f.First);
//                 Check(tool.CreateCommand(data, GridBuilder.Pixel(Vector2Int.zero), f.Bag, out var command, out var error), error);
//                 Check(data.Modified.Count == 0 && !anchor.Floor && f.Bag.Currency == 20, "Preparation leaves grid, dirty set and payment untouched");
//                 Check(command.Do(out error), error);
//                 Check(ReferenceEquals(anchor, data[0, 0]) && anchor.Floor == f.First, "Commit retains live cell identity");
//                 Check(data.Modified.Count == 15 && data.Modified.Contains(data[-1, 2]), "Full footprint outside brush is dirty");
//                 var applied = data.Clone();
//                 data.Modified.Clear();
//                 Check(!command.Do(out _) && data.Modified.Count == 0 && f.Bag.Currency == 19, "Duplicate Do is rejected without payment");
//                 Fixture.Set(f.First, "floor", f.Second.Floor);
//                 Check(command.Undo(out error), error);
//                 Check(f.Bag.Currency == 20 && data.Modified.Count == 15, "Undo refunds recorded payment and dirties restored footprint");
//                 foreach (var p in data.Area2.allPositionsWithin)
//                     Check(data[p.x, p.y].SameState(original[p.x, p.y]), "Undo restores exact original state");
//                 data.Modified.Clear();
//                 Check(command.Do(out error), error);
//                 Check(f.Bag.Currency == 19, "Redo reapplies recorded payment");
//                 foreach (var p in data.Area2.allPositionsWithin)
//                     Check(data[p.x, p.y].SameState(applied[p.x, p.y]), "Redo restores recorded states without rerunning tool");
//             }, completed);
//             Case("rejected command preparation and payment isolation", f =>
//             {
//                 var data = f.Builder.Snapshot();
//                 var before = data.Clone();
//                 var tool = new BaseLayerPlaceTool(f.First);
//                 Check(!tool.CreateCommand(data, GridBuilder.Pixel(new Vector2Int(-4, -6)), f.Bag, out _, out _), "Invalid footprint rejected while staging");
//                 Check(data.Modified.Count == 0 && f.Bag.Currency == 20, "Failed preparation has no dirty cells or payment");
//                 Check(tool.CreateCommand(data, GridBuilder.Pixel(Vector2Int.zero), f.Bag, out var command, out var error), error);
//                 Check(f.Bag.TryApply(Spend(20), out _), "Exhaust currency after preparing command");
//                 Check(!command.Do(out _) && data.Modified.Count == 0, "Failed payment leaves dirty set unchanged");
//                 foreach (var p in data.Area2.allPositionsWithin)
//                     Check(data[p.x, p.y].SameState(before[p.x, p.y]), "Rejected command leaves live cells unchanged");
//             }, completed);
//             Case("dirty flush, topping identity and failed undo", f =>
//             {
//                 var data = f.LiveData;
//                 Check(data.Modified.Count == 0, "Initialization flushes dirty cells");
//                 Check(f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.zero), f.Chair), f.Builder.LastError);
//                 Check(f.Builder.TryPlaceTopping(Vector2Int.zero, f.Cup, 0), f.Builder.LastError);
//                 var toppingRoot = f.Builder.BaseMap.transform.Find("Runtime Toppings");
//                 var unchanged = toppingRoot.GetChild(0).gameObject;
//                 Check(f.Builder.TryPlaceFloor(GridBuilder.Pixel(new Vector2Int(5, 0)), f.First), f.Builder.LastError);
//                 Check(unchanged && toppingRoot.childCount == 1 && toppingRoot.GetChild(0).gameObject == unchanged, "Unrelated edit preserves topping instance");
//                 Check(f.Builder.TryPlaceTopping(Vector2Int.zero, f.Cup, 1), f.Builder.LastError);
//                 Check(toppingRoot.GetChild(0).gameObject == unchanged, "Topping-only edit preserves unchanged slot instance");
//                 Check(data.Modified.Count == 0, "Successful rendering flushes modified cells");
//                 Check(!f.Builder.TryMove(GridBuilder.Pixel(Vector2Int.zero), new Vector2Int(-20, 0)), "Invalid move rejected");
//                 Check(data.Modified.Count == 0 && f.Inventory.Count(f.Cup) == 4, "Failed move preserves dirty set and inventory");
//                 f.Inventory.GetStackSize = _ => 4;
//                 Check(!f.Builder.Undo() && f.Builder.CanUndo && data.Modified.Count == 0, "Failed undo preserves history and dirty set");
//                 f.Inventory.GetStackSize = _ => int.MaxValue;
//                 Check(f.Builder.Undo() && data.Modified.Count == 0, "Successful undo flushes changed cells");
//             }, completed);
//             Case("load replaces rendered bounds and rebinds cells", f =>
//             {
//                 Check(f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.zero), f.Chair), f.Builder.LastError);
//                 Check(f.Builder.TryPlaceTopping(Vector2Int.zero, f.Cup), f.Builder.LastError);
//                 var root = new GameObject("Alternate bounds", typeof(Grid));
//                 try
//                 {
//                     var mapObject = new GameObject("Alternate source", typeof(Tilemap));
//                     mapObject.transform.SetParent(root.transform, false);
//                     var map = mapObject.GetComponent<Tilemap>();
//                     var at = new Vector3Int(30, 40, 2);
//                     map.SetTile(at, f.Grass);
//                     map.CompressBounds();
//                     var replacement = new GridData(map);
//                     using var stream = new MemoryStream();
//                     using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) replacement.Save(writer);
//                     stream.Position = 0;
//                     using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) f.Builder.Load(reader);
//                     Check(f.Builder.BaseMap.GetTile(at) == f.Grass && !f.Builder.BaseMap.GetTile(Vector3Int.zero), "Load clears old base bounds");
//                     Check(!f.Builder.FurnitureMap.GetTile(Vector3Int.zero), "Load clears old furniture");
//                     Check(f.LiveData.Origin2 == at && f.LiveData.Width == 1 && f.LiveData.Height == 1, "Load adopts new bounds and Z plane");
//                     var cell = f.LiveData[30, 40];
//                     Check(cell.Grid == f.LiveData && cell.Position == new Vector2Int(30, 40), "Loaded cell links to live grid");
//                     Check(f.LiveData.Modified.Count == 0 && !f.Builder.CanUndo && !f.Builder.CanRedo, "Load flushes dirty cells and history");
//                 }
//                 finally { Object.DestroyImmediate(root); }
//             }, completed);
//             Case("brush geometry and tool dispatch", f =>
//             {
//                 IBuildBrush pixel = new BuildPixelBrush();
//                 IBuildBrush rectangle = new BuildRectBrush();
//                 var first = new Vector2Int(2, 1); var last = new Vector2Int(-1, -2);
//                 Check(pixel.GetArea(first, last) == GridBuilder.Pixel(last), "Pixel follows current endpoint");
//                 Check(rectangle.GetArea(first, last) == new RectInt(-1, -2, 4, 4), "Rectangle includes reversed endpoints");
//                 Check(rectangle.GetArea(first, first) == GridBuilder.Pixel(first), "Single-cell rectangle");
//                 IBuildTool place = new BaseLayerPlaceTool(f.First);
//                 Check(f.Builder.TryUseTool(GridBuilder.Pixel(Vector2Int.zero), place), f.Builder.LastError);
//                 Check(f.Bag.Currency == 19 && f.Builder.CanUndo, "Tool dispatch uses payment and history");
//                 Check(f.Builder.Undo() && f.Bag.Currency == 20, "Tool dispatch undo refunds payment");
//                 Check(!f.Builder.TryUseTool(new RectInt(0, 0, 2, 1), new ToppingLayerPlaceTool(f.Cup)), "Topping tool rejects non-pixel areas");
//                 Check(f.Inventory.Count(f.Cup) == 6 && f.Bag.Currency == 20, "Rejected tool does not spend inventory");
//             }, completed);
//             Case("initialization and mixed default terrain", f =>
//             {
//                 var data = f.Builder.Snapshot();
//                 Check(data.Width == 14 && data.Height == 12 && data.Origin2 == new Vector3Int(-4, -6, 0), "Source bounds/origin");
//                 Check(data[-4, -6].DefaultTile == f.Stone && data[-3, -5].DefaultTile == null, "Mixed and empty defaults");
//                 Check(!f.Source.GetComponent<TilemapRenderer>().enabled, "Setup renderer is hidden");
//                 Check(f.Builder.BaseMap.GetTile(new Vector3Int(-4, -6, 0)) == f.Stone, "Initial terrain is rendered");
//                 Check(!f.Builder.TryPaint(GridBuilder.Pixel(Vector2Int.zero), f.Second), "Defaults cannot be painted");
//                 Check(!f.Builder.TryRemove(GridBuilder.Pixel(Vector2Int.zero), BuildLayer.Base), "Defaults cannot be refunded");
//                 Check(f.Bag.Currency == 20, "Defaults do not change currency");
//             }, completed);
//             Case("floor pattern, boundary rejection, paint and restore", f =>
//             {
//                 Check(!f.Builder.TryPlaceFloor(GridBuilder.Pixel(new Vector2Int(-4, -6)), f.First), "Pattern cannot cross bounds");
//                 Check(f.Bag.Currency == 20 && !f.Builder.CanUndo, "Rejected pattern is atomic");
//                 var at = new Vector2Int(2, 0);
//                 Check(f.Builder.TryPlaceFloor(GridBuilder.Pixel(at), f.First), f.Builder.LastError);
//                 Check(f.Bag.Currency == 19, "One floor cost");
//                 var rows = new[] { "RRR", "RWR", "RFR", "RRR", "WWW" };
//                 var data = f.Builder.Snapshot();
//                 for (var row = 0; row < 5; row++)
//                 for (var x = 0; x < 3; x++)
//                 {
//                     var type = ((DbAutoTile)data[at.x + x - 1, at.y + 2 - row].BaseTile).Type;
//                     Check(type == (rows[row][x] == 'F' ? AutoTileType.Floor : rows[row][x] == 'W' ? AutoTileType.Wall : AutoTileType.Roof), "Exact 3 x 5 pattern");
//                 }
//                 Check(!f.Builder.TryPlaceFloor(GridBuilder.Pixel(at), f.First), "Repeated anchor is not charged");
//                 Check(f.Builder.TryPaint(GridBuilder.Pixel(at), f.Second), f.Builder.LastError);
//                 data = f.Builder.Snapshot();
//                 Check(data[2, 0].BaseTile == f.Second.Floor && data[2, 1].BaseTile == f.First.Wall, "Paint affects brush only");
//                 Check(f.Bag.Currency == 19, "Paint is free");
//                 Check(f.Builder.TryRemove(GridBuilder.Pixel(at), BuildLayer.Base), f.Builder.LastError);
//                 Check(f.Bag.Currency == 20, "Refund uses floor definition cost");
//                 var restored = f.Builder.Snapshot();
//                 foreach (var p in restored.Area2.allPositionsWithin)
//                     Check(restored[p.x, p.y].BaseTile == restored[p.x, p.y].DefaultTile, "Per-cell terrain restored");
//             }, completed);
//             Case("per-cell restoration across mixed terrain and changed setup", f =>
//             {
//                 var at = new Vector2Int(-3, -4);
//                 Check(f.Builder.TryPlaceFloor(GridBuilder.Pixel(at), f.First), f.Builder.LastError);
//                 using var stream = new MemoryStream();
//                 using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) f.Builder.Save(writer);
//                 f.Source.SetTile(new Vector3Int(-4, -6, 0), f.Grass);
//                 f.Source.SetTile(new Vector3Int(-3, -5, 0), f.Stone);
//                 stream.Position = 0;
//                 using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) f.Builder.Load(reader);
//                 Check(f.Builder.TryRemove(GridBuilder.Pixel(at), BuildLayer.Base), f.Builder.LastError);
//                 var restored = f.Builder.Snapshot();
//                 Check(restored[-4, -6].BaseTile == f.Stone, "Stone cell restored from saved default, not changed setup");
//                 Check(restored[-3, -5].BaseTile == null, "Empty cell restored from saved null default");
//                 Check(f.Source.GetTile(new Vector3Int(-3, -5, 0)) == f.Stone, "Editing never changes setup tiles");
//             }, completed);
//             Case("overlapping floors and shared pattern ownership", f =>
//             {
//                 var a = new Vector2Int(1, 0); var b = new Vector2Int(2, 0);
//                 Check(f.Builder.TryPlaceFloor(GridBuilder.Pixel(a), f.First), f.Builder.LastError);
//                 Check(f.Builder.TryPlaceFloor(GridBuilder.Pixel(b), f.Second), f.Builder.LastError);
//                 Check(f.Builder.Snapshot()[1, 0].BaseTile == f.First.Floor, "Existing floor survives neighbor placement");
//                 Check(f.Builder.TryRemove(GridBuilder.Pixel(a), BuildLayer.Base), f.Builder.LastError);
//                 var state = f.Builder.Snapshot();
//                 Check(state[2, 0].Floor == f.Second && state[1, 0].Generated, "Surviving floor still owns shared pattern cells");
//                 Check(state[0, 0].BaseTile == f.Grass && f.Bag.Currency == 19, "Unneeded cells restored and one refund");
//             }, completed);
//             Case("atomic rectangle costs and undo/redo", f =>
//             {
//                 var rectangle = GridBuilder.Rectangle(new Vector2Int(1, 0), new Vector2Int(2, 0));
//                 Check(f.Builder.TryPlaceFloor(rectangle, f.First), f.Builder.LastError);
//                 Check(f.Bag.Currency == 18, "Rectangle costs two floors");
//                 Check(f.Builder.Undo() && f.Bag.Currency == 20, "Undo refunds full rectangle");
//                 Check(f.Builder.Redo() && f.Bag.Currency == 18, "Redo pays full rectangle");
//                 Check(f.Bag.TryApply(Spend(18), out _), "Spend remaining currency externally");
//                 Check(!f.Builder.TryPlaceFloor(GridBuilder.Pixel(new Vector2Int(5, 0)), f.First), "Insufficient currency rejects edit");
//                 Check(!f.Builder.Snapshot()[5, 0].Floor && f.Bag.Currency == 0, "Failed edit leaves grid and balances intact");
//                 Check(f.Builder.Undo() && f.Bag.Currency == 2, "Undo still refunds paid command");
//                 Check(f.Bag.TryApply(Spend(2), out _), "Spend refunded currency externally");
//                 Check(!f.Builder.Redo() && f.Builder.CanRedo && !f.Builder.Snapshot()[1, 0].Floor, "Unaffordable redo keeps history and grid");
//                 Check(f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.zero), f.Chair), f.Builder.LastError);
//                 Check(!f.Builder.CanRedo, "New edit clears redo");
//             }, completed);
//             Case("furniture, topping slots, nearest removal and group move", f =>
//             {
//                 Check(f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.zero), f.Chair), f.Builder.LastError);
//                 Check(f.Inventory.Count(f.Chair) == 4, "Furniture consumes item on default Floor");
//                 Check(f.Builder.TryPlaceTopping(Vector2Int.zero, f.Cup, 0), f.Builder.LastError);
//                 Check(f.Builder.TryPlaceTopping(Vector2Int.zero, f.Cup, 1), f.Builder.LastError);
//                 Check(!f.Builder.TryPlaceTopping(Vector2Int.zero, f.Cup), "Full slots reject placement");
//                 var right = f.Builder.BaseMap.GetCellCenterWorld(Vector3Int.zero) + new Vector3(.25f, 0, 0);
//                 Check(f.Builder.TryRemove(GridBuilder.Pixel(Vector2Int.zero), BuildLayer.Topping, right), f.Builder.LastError);
//                 Check(f.Builder.Snapshot()[0, 0].Toppings[0] && !f.Builder.Snapshot()[0, 0].Toppings[1], "Nearest slot removed");
//                 Check(f.Builder.Undo() && f.Builder.Snapshot()[0, 0].Toppings[1], "Undo restores slot");
//                 Check(f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.right), f.Chair), f.Builder.LastError);
//                 Check(f.Builder.TryMove(GridBuilder.Rectangle(Vector2Int.zero, Vector2Int.right), Vector2Int.right), f.Builder.LastError);
//                 var state = f.Builder.Snapshot();
//                 Check(!state[0, 0].Furniture && state[1, 0].Toppings[0] && state[1, 0].Toppings[1] && state[2, 0].Furniture, "Overlapping group move carries slots");
//                 Check(f.Builder.TryRemove(GridBuilder.Pixel(Vector2Int.right), BuildLayer.Furniture), f.Builder.LastError);
//                 Check(f.Inventory.Count(f.Cup) == 6 && f.Inventory.Count(f.Chair) == 4, "Furniture removal refunds slotted toppings");
//                 Check(f.Builder.Undo() && f.Inventory.Count(f.Cup) == 4, "Undo removal consumes returned toppings");
//             }, completed);
//             Case("BuildOn and occupied footprint rejection", f =>
//             {
//                 Check(f.Builder.TryPlaceFloor(GridBuilder.Pixel(Vector2Int.zero), f.First), f.Builder.LastError);
//                 Check(!f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.up), f.Chair), "Floor furniture cannot be placed on Wall");
//                 Check(f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.zero), f.Chair), f.Builder.LastError);
//                 Check(!f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.zero), f.Chair), "Occupied destination rejected");
//                 Check(!f.Builder.TryRemove(GridBuilder.Pixel(Vector2Int.zero), BuildLayer.Base), "Furniture blocks base removal");
//                 Check(!f.Builder.TryMove(GridBuilder.Pixel(Vector2Int.zero), Vector2Int.up), "Move validates destination BuildOn");
//                 Check(f.Builder.Snapshot()[0, 0].Furniture == f.Chair && f.Inventory.Count(f.Chair) == 4, "Rejected moves do not consume items or move furniture");
//                 Check(!f.Builder.TryPlaceFurniture(new RectInt(4, 0, 6, 2), f.Chair), "Insufficient items reject entire rectangle");
//                 Check(!f.Builder.Snapshot()[4, 0].Furniture && f.Inventory.Count(f.Chair) == 4, "Failed rectangle is atomic");
//             }, completed);
//             Case("binary round trip, defaults and corrupt-load isolation", f =>
//             {
//                 Check(f.Builder.TryPlaceFloor(GridBuilder.Pixel(Vector2Int.zero), f.First), f.Builder.LastError);
//                 Check(f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.zero), f.Chair), f.Builder.LastError);
//                 Check(f.Builder.TryPlaceTopping(Vector2Int.zero, f.Cup, 1), f.Builder.LastError);
//                 Check(f.Builder.TryPaint(GridBuilder.Pixel(Vector2Int.zero), f.Second), f.Builder.LastError);
//                 using var stream = new MemoryStream();
//                 using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) f.Builder.Save(writer);
//                 Check(f.Builder.TryMove(GridBuilder.Pixel(Vector2Int.zero), new Vector2Int(4, 0)), f.Builder.LastError);
//                 stream.Position = 0;
//                 using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) f.Builder.Load(reader);
//                 var data = f.Builder.Snapshot();
//                 Check(data.Origin2 == new Vector3Int(-4, -6, 0) && data.Width == 14 && data.Height == 12, "Dimensions round trip");
//                 Check(data[0, 0].BaseTile == f.Second.Floor && data[0, 0].Furniture == f.Chair && data[0, 0].Toppings[1] == f.Cup, "Definitions and slots round trip");
//                 Check(data[-4, -6].DefaultTile == f.Stone && data[-3, -5].DefaultTile == null, "Mixed and null defaults round trip");
//                 Check(!f.Builder.CanUndo && !f.Builder.CanRedo, "Load clears history");
//                 var bytes = stream.ToArray(); bytes[0] = 99;
//                 var rejected = false;
//                 try { using var broken = new BinaryReader(new MemoryStream(bytes)); f.Builder.Load(broken); }
//                 catch (InvalidDataException) { rejected = true; }
//                 Check(rejected && f.Builder.Snapshot()[0, 0].Furniture == f.Chair, "Corrupt load leaves grid unchanged");
//                 Check(f.Builder.TryRemove(GridBuilder.Pixel(Vector2Int.zero), BuildLayer.Furniture), f.Builder.LastError);
//                 Check(f.Builder.TryRemove(GridBuilder.Pixel(Vector2Int.zero), BuildLayer.Base), f.Builder.LastError);
//                 Check(f.Builder.Snapshot()[0, 0].BaseTile == f.Grass, "Restoration still works after load");
//             }, completed);
//             Case("missing save keys and truncated load", f =>
//             {
//                 Fixture.Set(f.Grass, "item", null);
//                 using var stream = new MemoryStream();
//                 var rejected = false;
//                 try { using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true); f.Builder.Save(writer); }
//                 catch (InvalidDataException) { rejected = true; }
//                 Check(rejected && stream.Length == 0, "Missing key rejects save before writing caller stream");
//                 rejected = false;
//                 try { using var reader = new BinaryReader(new MemoryStream(new byte[] { 2, 0, 0, 0 })); f.Builder.Load(reader); }
//                 catch (EndOfStreamException) { rejected = true; }
//                 Check(rejected && f.Builder.Snapshot()[0, 0].BaseTile == f.Grass, "Truncated load preserves live grid");
//                 Check(f.Builder.TryPlaceFloor(GridBuilder.Pixel(Vector2Int.zero), f.First), "Editing remains possible after rejected persistence");
//             }, completed);
//             Case("inventory capacity and failed undo", f =>
//             {
//                 Check(f.Builder.TryPlaceFurniture(GridBuilder.Pixel(Vector2Int.zero), f.Chair), f.Builder.LastError);
//                 f.Inventory.GetStackSize = _ => 4;
//                 Check(!f.Builder.TryRemove(GridBuilder.Pixel(Vector2Int.zero), BuildLayer.Furniture), "Refund capacity checked");
//                 Check(!f.Builder.Undo() && f.Builder.CanUndo && f.Builder.Snapshot()[0, 0].Furniture, "Failed refund undo leaves command intact");
//                 f.Inventory.GetStackSize = _ => int.MaxValue;
//                 Check(f.Builder.Undo() && f.Inventory.Count(f.Chair) == 5, "Undo succeeds after capacity restored");
//             }, completed);
//             return $"TileGrid validation passed: {completed.Count} scenarios, {assertions} assertions.\n" + string.Join("\n", completed);
//         }
//
//         private static BuildPayment Spend(int amount)
//         {
//             var payment = new BuildPayment(); payment.AddCurrency(-amount); return payment;
//         }
//
//         private static void Case(string name, Action<Fixture> test, List<string> completed)
//         {
//             using var fixture = new Fixture();
//             try { test(fixture); completed.Add(name); }
//             catch (Exception e) { throw new InvalidOperationException($"TileGrid validation failed: {name}: {e.Message}", e); }
//         }
//
//         private static void Check(bool condition, string message)
//         {
//             assertions++;
//             if (!condition) throw new InvalidOperationException(message ?? "Unexpected operation failure.");
//         }
//
//         private sealed class RefreshProbeTile : Tile
//         {
//             public int Reads;
//             public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
//             {
//                 Reads++;
//                 base.GetTileData(position, tilemap, ref tileData);
//             }
//         }
//
//         private sealed class Fixture : IDisposable
//         {
//             private readonly Scene scene;
//             private readonly List<Object> assets = new();
//             private readonly FieldInfo databaseField = typeof(Game).GetField("_database", BindingFlags.Static | BindingFlags.NonPublic);
//             private readonly object previousDatabase;
//             private readonly GameObject root;
//             public readonly GridBuilder Builder;
//             public GridData LiveData => (GridData)typeof(GridBuilder).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Builder);
//             public readonly Tilemap Source;
//             public readonly BaseTileSo First, Second;
//             public readonly DbAutoTile Grass;
//             public readonly DbTile Stone;
//             public readonly FurnitureTileSo Chair;
//             public readonly ToppingTileSo Cup;
//             public readonly ListInventory<DatabaseEntrySo> Inventory = new();
//             public readonly InventoryBuildBag Bag;
//
//             public Fixture()
//             {
//                 previousDatabase = databaseField.GetValue(null);
//                 scene = EditorSceneManager.NewPreviewScene();
//                 root = new GameObject("TileGrid validation", typeof(Grid));
//                 SceneManager.MoveGameObjectToScene(root, scene);
//                 var source = new GameObject("Setup", typeof(Tilemap), typeof(TilemapRenderer));
//                 source.transform.SetParent(root.transform, false);
//                 Source = source.GetComponent<Tilemap>();
//                 var terrain = MakeFloor(105); Grass = terrain.Floor;
//                 var stoneDefinition = Asset<FurnitureTileSo>(); Set(stoneDefinition, "id", 106u); Stone = Asset<DbTile>(); Set(Stone, "item", stoneDefinition); Set(stoneDefinition, "tile", Stone);
//                 for (var y = -6; y < 6; y++)
//                 for (var x = -4; x < 10; x++) Source.SetTile(new Vector3Int(x, y, 0), Grass);
//                 Source.SetTile(new Vector3Int(-4, -6, 0), Stone);
//                 Source.SetTile(new Vector3Int(-3, -5, 0), null);
//                 First = MakeFloor(101); Second = MakeFloor(102);
//                 Chair = Asset<FurnitureTileSo>(); Set(Chair, "id", 103u); var chairTile = Asset<DbTile>(); Set(chairTile, "item", Chair); Set(Chair, "tile", chairTile);
//                 Set(Chair, "buildOn", AutoTileType.Floor);
//                 Set(Chair, "toppingSlots", new[] { new Vector2(-.25f, 0), new Vector2(.25f, 0) });
//                 Cup = Asset<ToppingTileSo>(); Set(Cup, "id", 104u);
//                 var prefab = new GameObject("Test topping"); prefab.transform.SetParent(root.transform, false); prefab.SetActive(false);
//                 Set(Cup, "prefab", prefab);
//                 var database = Asset<Systems.Database>();
//                 var populate = database.GetType().BaseType.GetMethod("__SetItems", BindingFlags.Instance | BindingFlags.NonPublic);
//                 populate.Invoke(database, new object[] { new DatabaseEntrySo[] { First, Second, Chair, Cup, terrain, stoneDefinition } });
//                 databaseField.SetValue(null, database);
//                 Inventory.Add(Chair, 5); Inventory.Add(Cup, 6);
//                 Bag = new InventoryBuildBag(Inventory, 20);
//                 Builder = root.AddComponent<GridBuilder>();
//                 Builder.Initialize(Source, Bag);
//             }
//
//             private BaseTileSo MakeFloor(uint id)
//             {
//                 var definition = Asset<BaseTileSo>(); Set(definition, "id", id);
//                 foreach (var type in new[] { AutoTileType.Floor, AutoTileType.Wall, AutoTileType.Roof })
//                 {
//                     var tile = Asset<DbAutoTile>(); tile.__setup(definition, type); Set(tile, "item", definition);
//                     Set(definition, type.ToString().ToLowerInvariant(), tile);
//                 }
//                 return definition;
//             }
//
//             private T Asset<T>() where T : ScriptableObject
//             {
//                 var asset = ScriptableObject.CreateInstance<T>(); assets.Add(asset); return asset;
//             }
//
//             public static void Set(object instance, string field, object value)
//             {
//                 for (var type = instance.GetType(); type != null; type = type.BaseType)
//                 {
//                     var info = type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
//                     if (info == null) continue;
//                     info.SetValue(instance, value); return;
//                 }
//                 throw new MissingFieldException(field);
//             }
//
//             public void Dispose()
//             {
//                 if (root) Object.DestroyImmediate(root);
//                 EditorSceneManager.ClosePreviewScene(scene);
//                 databaseField.SetValue(null, previousDatabase);
//                 foreach (var asset in assets) if (asset) Object.DestroyImmediate(asset);
//             }
//         }
//     }
// }
// #endif
