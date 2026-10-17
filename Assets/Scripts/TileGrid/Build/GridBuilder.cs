using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Drafts.SaveData;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Runtime building API. Input controllers call the brush methods in cell coordinates.</summary>
    public sealed class GridBuilder : MonoBehaviour, IBinarySave
    {
        [SerializeField] private Tilemap initialTilemap;
        [SerializeField] private MonoBehaviour buildBag;
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("buildingMaterial")]
        private DatabaseEntrySo material;
        [SerializeField, Min(1)] private int historyLimit = 64;

        private Tilemap _baseMap;
        private Tilemap _furnitureMap;
        private Transform _toppingRoot;
        private TilemapRenderer _sourceRenderer;
        private bool _sourceWasVisible;
        private IBuildBag _bag;
        private GridData _data;
        private readonly List<IBuildCommand> _undo = new();
        private readonly List<IBuildCommand> _redo = new();
        private bool _editing;

        public GridData Grid => _data;
        public Vector3Int Origin => _data.Bounds.position;
        public IBuildBag Bag => _bag;
        public bool IsInitialized => _data != null;
        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;
        public Tilemap BaseMap => _baseMap;
        public Tilemap FurnitureMap => _furnitureMap;
        public DatabaseEntrySo Material => material;

        public IReadOnlyList<T> GetAvailable<T>() where T : DatabaseEntrySo
        {
            if (_bag == null) return Array.Empty<T>();
            if (typeof(T) == typeof(BaseTileSo)) return _bag.GetUnlockedTiles().Cast<T>().ToArray();
            return _bag.GetAvailableItems<T>().Where(pair => pair.Item2 > 0)
                .Select(pair => pair.Item1).ToArray();
        }

        public string LastError { get; private set; }
        public event Action Changed;

        private sealed class RenderedTopping
        {
            public ToppingTileSo Definition;
            public GameObject Instance;
        }

        private readonly Dictionary<Vector2Int, RenderedTopping[]> _toppings = new();
        private readonly HashSet<Vector3Int> _baseRefresh = new();
        private readonly HashSet<Vector3Int> _furnitureRefresh = new();

        private void Start()
        {
            if (!IsInitialized && initialTilemap)
                Initialize(initialTilemap, buildBag as IBuildBag);
        }

        public void Initialize(Tilemap source, IBuildBag inventory)
        {
            if (IsInitialized) throw new InvalidOperationException("GridBuilder is already initialized.");
            if (inventory == null) throw new ArgumentNullException(nameof(inventory));
            if (!source || !source.layoutGrid) throw new ArgumentException("Source Tilemap needs a parent Grid.");
            _bag = inventory;
            initialTilemap = source;
            _data = new GridData(source);
            _baseMap = CreateMap("Runtime Base", source);
            _furnitureMap = CreateMap("Runtime Furniture", source);
            var root = new GameObject("Runtime Toppings");
            root.transform.SetParent(_baseMap.transform, false);
            _toppingRoot = root.transform;
            _sourceRenderer = source.GetComponent<TilemapRenderer>();
            _sourceWasVisible = _sourceRenderer && _sourceRenderer.enabled;
            if (_sourceRenderer) _sourceRenderer.enabled = false;
            Render();
        }

        private static readonly IBuildBrush PixelBrush = new BuildPixelBrush();
        private static readonly IBuildBrush RectBrush = new BuildRectBrush();

        public static RectInt Pixel(Vector2Int cell) => PixelBrush.GetArea(cell, cell);

        /// <summary>Both rectangle endpoints are included.</summary>
        public static RectInt Rectangle(Vector2Int first, Vector2Int last) => RectBrush.GetArea(first, last);

        private readonly BaseLayerPlaceTool _placeFloor = new();
        // private readonly BaseLayerPaintTool _paint = new();
        // private readonly FurnitureLayerPlaceTool _placeFurniture = new();
        // private readonly ToppingLayerPlaceTool _placeTopping = new();
        // private readonly BaseLayerRemoveTool _removeBase = new();
        // private readonly FurnitureLayerRemoveTool _removeFurniture = new();
        // private readonly ToppingLayerRemoveTool _removeTopping = new();
        // private readonly FurnitureLayerMoveTool _moveFurniture = new();

        public bool TryPlaceFloor(RectInt area, BaseTileSo tile) =>
            TryUseTool(area, _placeFloor, tile);

        public bool TryPaint(RectInt area, BaseTileSo tile) => Fail("Painting is temporarily disabled.");

        public bool TryPlaceFurniture(RectInt area, FurnitureTileSo item) =>
            Fail("Furniture placement is temporarily disabled.");

        /// <summary>Slot -1 chooses the first free slot; positions are local offsets from the furniture cell center.</summary>
        public bool TryPlaceTopping(Vector2Int cell, ToppingTileSo item, int slot = -1) =>
            Fail("Topping placement is temporarily disabled.");

        /// <summary>One topping per selected cell. Null pointer removes the first occupied slot.</summary>
        public bool TryRemove(RectInt area, BuildLayer layer, Vector3? worldPointer = null) =>
            Fail("Removal is temporarily disabled.");

        /// <summary>Runs a tool through inventory checks, atomic commit, rendering and undo history.</summary>
        public bool TryUseTool(RectInt area, IBuildTool tool, object item = null, int slot = -1,
            Vector2Int delta = default, Func<Vector2Int, int> nearestSlot = null)
        {
            if (tool == null) throw new ArgumentNullException(nameof(tool));
            if (!Ready()) return false;
            _editing = true;

            try
            {
                var args = new OperationArgs
                {
                    grid = _data,
                    bag = _bag,
                    area = area,
                    item = item,
                    builder = this,
                    slot = slot,
                    delta = delta,
                    nearestSlot = nearestSlot
                };
                var command = tool.CreateCommand(args, out var error);
                if (command == null) return Fail(error);
                if (!command.Do()) return Fail("The command could not be applied.");
                _undo.Add(command);
                if (_undo.Count > Mathf.Max(1, historyLimit)) _undo.RemoveAt(0);
                _redo.Clear();
                LastError = null;
                Render();
            }
            finally
            {
                _editing = false;
            }

            Changed?.Invoke();
            return true;
        }

        /// <summary>Moves all selected furniture by the same cell offset, including attached toppings. Free.</summary>
        public bool TryMove(RectInt area, Vector2Int delta) =>
            Fail("Furniture movement is temporarily disabled.");

        public bool Undo() => Restore(_undo, _redo, true);
        public bool Redo() => Restore(_redo, _undo, false);

        private bool Restore(List<IBuildCommand> from, List<IBuildCommand> to, bool reverse)
        {
            if (!Ready()) return false;
            if (from.Count == 0) return Fail("No command is available.");
            _editing = true;

            try
            {
                var command = from[from.Count - 1];
                var success = reverse ? command.Undo() : command.Do();
                if (!success) return Fail("The command could not be restored.");
                from.RemoveAt(from.Count - 1);
                to.Add(command);
                LastError = null;
                Render();
            }
            finally
            {
                _editing = false;
            }

            Changed?.Invoke();
            return true;
        }

        public Vector2Int WorldToCell(Vector3 world)
        {
            if (!IsInitialized) throw new InvalidOperationException("Grid is not initialized.");
            var cell = _baseMap.WorldToCell(world);
            return new Vector2Int(cell.x - _data.Bounds.x, cell.y - _data.Bounds.y);
        }

        private int NearestEmptySlot(GridData state, Vector2Int p, Vector3? pointer)
        {
            var cell = state[p.x, p.y];
            if (!cell.Furniture) return -1;
            var used = cell.Toppings;

            if (!pointer.HasValue)
            {
                for (var i = 0; i < used.Count; i++)
                    if (!used[i])
                        return i;
                return -1;
            }

            var pos = pointer.Value - cell.Position; // move to local tile position
            var best = -1;
            var slots = cell.Furniture.ToppingSlots;
            var distance = float.PositiveInfinity;

            for (var i = 0; i < slots.Count; i++)
            {
                if (used[i]) continue;
                if (!pointer.HasValue) return i;
                var current = (SlotWorld(state, p, slots[i]) - pointer.Value).sqrMagnitude;

                if (current < distance)
                {
                    best = i;
                    distance = current;
                }
            }

            return best;
        }

        private Vector3 SlotWorld(GridData state, Vector2Int p, Vector2 offset) =>
            _baseMap.GetCellCenterWorld(new Vector3Int(p.x + state.Bounds.x, p.y + state.Bounds.y, state.Bounds.z)) +
            _baseMap.transform.TransformVector(new Vector3(offset.x, offset.y, 0));

        private void Render()
        {
            var dirty = _data.Modified.ToArray();

            foreach (var cell in dirty)
            {
                var p = cell.Key;
                var at = new Vector3Int(p.x + _data.Bounds.x, p.y + _data.Bounds.y, _data.Bounds.z);
                SetChangedTile(_baseMap, at, cell.BaseTile, _baseRefresh);
                SetChangedTile(_furnitureMap, at, cell.Furniture ? cell.Furniture.Tile : null, _furnitureRefresh);
                RenderToppings(cell);
            }

            // AutoTile uses the eight surrounding cells. Refresh after all writes are visible.
            foreach (var at in _baseRefresh) _baseMap.RefreshTile(at);
            foreach (var at in _furnitureRefresh) _furnitureMap.RefreshTile(at);
            // On failure, retain the dirty set so a later render can retry.
            _data.Modified.ExceptWith(dirty);
            _baseRefresh.Clear();
            _furnitureRefresh.Clear();
        }

        private static void SetChangedTile(Tilemap map, Vector3Int at, TileBase tile, HashSet<Vector3Int> refresh)
        {
            if (map.GetTile(at) == tile) return;
            map.SetTile(at, tile);
            for (var y = -1; y <= 1; y++)
            for (var x = -1; x <= 1; x++)
                refresh.Add(at + new Vector3Int(x, y, 0));
        }

        private void RenderToppings(GridCell cell)
        {
            _toppings.TryGetValue(cell.Key, out var rendered);
            var count = cell.Furniture ? cell.slots.Count : 0;
            if (rendered == null && count == 0) return;
            if (rendered == null) rendered = Array.Empty<RenderedTopping>();
            for (var i = count; i < rendered.Length; i++) DestroyTopping(rendered[i]);
            if (rendered.Length != count) Array.Resize(ref rendered, count);
            // Keep the cache current even if creating a topping fails partway through a cell.
            if (count == 0) _toppings.Remove(cell.Key);
            else _toppings[cell.Key] = rendered;

            for (var i = 0; i < count; i++)
            {
                var desired = cell.slots[i];
                var current = rendered[i];

                if (current != null && (current.Definition != desired || !current.Instance))
                {
                    DestroyTopping(current);
                    rendered[i] = current = null;
                }

                if (!desired) continue;

                if (current == null)
                {
                    var instance = Instantiate(desired.Prefab, _toppingRoot);
                    rendered[i] = current = new RenderedTopping { Definition = desired, Instance = instance };
                }

                current.Instance.transform.position = SlotWorld(_data, cell.Key, cell.Furniture.ToppingSlots[i]);
            }
        }

        private static void DestroyTopping(RenderedTopping topping)
        {
            if (topping == null || !topping.Instance) return;
            topping.Instance.SetActive(false);
            DisposeObject(topping.Instance);
        }

        private void ClearRendered()
        {
            _baseMap.ClearAllTiles();
            _furnitureMap.ClearAllTiles();
            foreach (var toppings in _toppings.Values)
            foreach (var topping in toppings)
                DestroyTopping(topping);
            _toppings.Clear();
            _baseRefresh.Clear();
            _furnitureRefresh.Clear();
        }

        private static Tilemap CreateMap(string name, Tilemap source)
        {
            var go = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
            go.transform.SetParent(source.transform.parent, false);
            go.transform.localPosition = source.transform.localPosition;
            go.transform.localRotation = source.transform.localRotation;
            go.transform.localScale = source.transform.localScale;
            go.layer = source.gameObject.layer;
            var map = go.GetComponent<Tilemap>();
            map.color = source.color;
            map.tileAnchor = source.tileAnchor;
            map.orientation = source.orientation;
            map.orientationMatrix = source.orientationMatrix;
            var original = source.GetComponent<TilemapRenderer>();

            if (original)
            {
                var renderer = go.GetComponent<TilemapRenderer>();
                renderer.sharedMaterials = original.sharedMaterials;
                renderer.sortingLayerID = original.sortingLayerID;
                renderer.sortingOrder = original.sortingOrder + (name == "Runtime Furniture" ? 1 : 0);
                renderer.sortOrder = original.sortOrder;
            }

            return map;
        }

        /// <summary>Writes only grid state. The owning save system must persist inventory separately.</summary>
        public void Save(BinaryWriter writer)
        {
            if (!IsInitialized || _editing) throw new InvalidOperationException("Grid is not ready to save.");
            // Stage serialization so a missing tile key cannot partially write the caller's stream.
            using var stream = new MemoryStream();
            using (var staged = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) _data.Save(staged, new GameGridSerializer());
            writer.Write(stream.ToArray());
        }

        public void Load(BinaryReader reader)
        {
            if (!Ready()) throw new InvalidOperationException(LastError);
            _editing = true;

            try
            {
                // Load the saved cells and reset history before rendering the replacement state.
                _data.Load(reader, new GameGridSerializer());
                _undo.Clear();
                _redo.Clear();
                LastError = null;
                ClearRendered();
                Render();
            }
            finally
            {
                _editing = false;
            }

            Changed?.Invoke();
        }

        public void SaveFile(string path)
        {
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true)) Save(writer);
            var temporary = path + ".tmp";
            File.WriteAllBytes(temporary, stream.ToArray());
            if (File.Exists(path)) File.Replace(temporary, path, null);
            else File.Move(temporary, path);
        }

        public void LoadFile(string path)
        {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);
            Load(reader);
        }

        private bool Ready() => IsInitialized && !_editing ||
                                Fail(IsInitialized ? "An edit is already in progress." : "Grid is not initialized.");

        private bool Fail(string error)
        {
            LastError = error;
            return false;
        }

        private void OnDestroy()
        {
            if (_sourceRenderer) _sourceRenderer.enabled = _sourceWasVisible;
            if (_baseMap) DisposeObject(_baseMap.gameObject);
            if (_furnitureMap) DisposeObject(_furnitureMap.gameObject);
        }

        private static void DisposeObject(UnityEngine.Object value)
        {
            if (Application.isPlaying) Destroy(value);
            else DestroyImmediate(value);
        }
    }
}