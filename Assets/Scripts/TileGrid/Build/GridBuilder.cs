using System;
using System.Collections.Generic;
using System.Linq;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class GridBuilder : MonoBehaviour
    {
        private static readonly GameGridSerializer Serializer = new();

        [SerializeField] private Tilemap defaultMap;
        [SerializeField] private MonoBehaviour buildBag;
        [SerializeField] private DatabaseEntrySo material;
        [SerializeField, Min(1)] private int historyLimit = 64;
        [SerializeField] private GridBuilderTools tools;
        [SerializeField] private string filePath;

        private readonly List<IBuildCommand> _undo = new();
        private readonly List<IBuildCommand> _redo = new();
        private OperationArgs _opArgs;

        public GridData Grid { get; private set; }
        public IBuildBag Bag { get; private set; }

        public LayerToolsConfig[] Tools => tools.LayerTools;
        public Vector3Int Origin => Grid.Bounds.position;
        public bool IsInitialized => Grid != null;
        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;
        public DatabaseEntrySo Material => material;
        
        public LayerToolsConfig CurrentTool { get; set; }
        public IBuildBrush CurrentBrush { get; set; }
        public object SelectedItem { get => _opArgs.item; set => _opArgs.item = value; }

        public IReadOnlyList<T> GetAvailable<T>() where T : DatabaseEntrySo
        {
            if (Bag == null) return Array.Empty<T>();
            if (typeof(T) == typeof(BaseTileSo)) return Bag.GetUnlockedTiles().Cast<T>().ToArray();
            return Bag.GetAvailableItems<T>().Where(pair => pair.Item2 > 0)
                .Select(pair => pair.Item1).ToArray();
        }

        public string LastError { get; private set; }
        public event Action Changed;

        private void Start()
        {
            _opArgs = new()
            {
                grid = Grid,
                bag = Bag,
                builder = this,
            };

            if (!defaultMap || !defaultMap.layoutGrid)
                throw new ArgumentException("Source Tilemap needs a parent Grid.");

            foreach (var tool in tools.LayerTools)
            {
                tool.Main.Builder = this;
                tool.Alt.Builder = this;
            }

            Grid = string.IsNullOrEmpty(filePath)
                ? new(defaultMap)
                : new GridData(Serializer, filePath);

            if (!IsInitialized && defaultMap)
                Initialize(defaultMap, buildBag as IBuildBag);
        }

        public void Initialize(Tilemap source, IBuildBag inventory)
        {
            if (IsInitialized) throw new InvalidOperationException("GridBuilder is already initialized.");
            Bag = inventory ?? throw new ArgumentNullException(nameof(inventory));
            defaultMap = source;
            Grid = new GridData(source);
        }

        /// <summary>Runs a tool through inventory checks, atomic commit, rendering and undo history.</summary>
        public bool TryUseTool(RectInt area, IBuildTool tool, int slot = -1,
            Vector2Int delta = default, Func<Vector2Int, int> nearestSlot = null)
        {
            if (tool == null) throw new ArgumentNullException(nameof(tool));

            _opArgs.area = area;
            _opArgs.slot = slot;
            _opArgs.delta = delta;
            _opArgs.nearestSlot = nearestSlot;

            var command = tool.CreateCommand(_opArgs, out var error);
            if (command == null) return Fail(error);
            if (!command.Do()) return Fail("The command could not be applied.");
            _redo.Clear();
            _undo.Add(command);
            if (_undo.Count > Mathf.Max(1, historyLimit)) _undo.RemoveAt(0);
            LastError = null;
            Changed?.Invoke();

            return true;
        }

        public bool Undo() => Restore(_undo, _redo, true);
        public bool Redo() => Restore(_redo, _undo, false);

        private bool Restore(List<IBuildCommand> from, List<IBuildCommand> to, bool reverse)
        {
            if (from.Count == 0) return Fail("No command is available.");

            var command = from[^1];
            var success = reverse ? command.Undo() : command.Do();
            if (!success) return Fail("The command could not be restored.");
            from.RemoveAt(from.Count - 1);
            to.Add(command);
            LastError = null;

            Changed?.Invoke();
            return true;
        }

        public Vector2Int WorldToCell(Vector3 world)
        {
            if (!IsInitialized) throw new InvalidOperationException("Grid is not initialized.");
            var cell = defaultMap.WorldToCell(world);
            return new Vector2Int(cell.x - Grid.Bounds.x, cell.y - Grid.Bounds.y);
        }

        // private int NearestEmptySlot(GridData state, Vector2Int p, Vector3? pointer)
        // {
        //     var cell = state[p.x, p.y];
        //     if (!cell.Furniture) return -1;
        //     var used = cell.Toppings;
        //
        //     if (!pointer.HasValue)
        //     {
        //         for (var i = 0; i < used.Count; i++)
        //             if (!used[i])
        //                 return i;
        //         return -1;
        //     }
        //
        //     var pos = pointer.Value - cell.Position; // move to local tile position
        //     var best = -1;
        //     var slots = cell.Furniture.ToppingSlots;
        //     var distance = float.PositiveInfinity;
        //
        //     for (var i = 0; i < slots.Count; i++)
        //     {
        //         if (used[i]) continue;
        //         if (!pointer.HasValue) return i;
        //         var current = (SlotWorld(state, p, slots[i]) - pointer.Value).sqrMagnitude;
        //
        //         if (current < distance)
        //         {
        //             best = i;
        //             distance = current;
        //         }
        //     }
        //
        //     return best;
        // }

        // private Vector3 SlotWorld(GridData state, Vector2Int p, Vector2 offset) =>
        //     defaultMap.GetCellCenterWorld(new Vector3Int(p.x + state.Bounds.x, p.y + state.Bounds.y, state.Bounds.z)) +
        //     defaultMap.transform.TransformVector(new Vector3(offset.x, offset.y, 0));

        public void Clear()
        {
            _undo.Clear();
            _redo.Clear();
            LastError = null;
            Changed?.Invoke();
        }

        public void SaveFile(string path)
        {
            filePath = path ?? filePath;
            if (string.IsNullOrEmpty(filePath))
                throw new Exception("no filePath");
            Grid?.Save(Serializer, filePath);
        }

        public void LoadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("no filePath");
            Grid = new(Serializer, filePath = path);
        }

        private bool Fail(string error)
        {
            LastError = error;
            return false;
        }
    }
}