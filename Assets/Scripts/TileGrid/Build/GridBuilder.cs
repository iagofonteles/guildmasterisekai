using System;
using System.Collections.Generic;
using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class GridBuilder : MonoBehaviour
    {
        [SerializeReference, TypeInstance] private IGridSerializer serializer;
        [SerializeField] private Tilemap[] defaultMaps;
        [SerializeField] private MonoBehaviour buildBag;
        [SerializeField] private ScriptableObject material;
        [SerializeField, Min(1)] private int historyLimit = 64;
        [SerializeField, SO] private GridBuilderTools tools;
        [SerializeField] private string filePath;

        private readonly List<IBuildCommand> _undo = new();
        private readonly List<IBuildCommand> _redo = new();
        private readonly OperationArgs _opArgs = new();

        public GridData Grid => _opArgs.grid;
        public IBuildBag Bag => _opArgs.bag;
        public LayerToolConfig CurrentTool { get; set; }
        public IBuildBrush CurrentBrush { get; set; }
        public string LastError { get; private set; }
        public event Action Changed;

        public LayerToolsConfig[] Tools => tools.LayerTools;
        public IReadOnlyList<IBuildBrush> Brushes => tools.Bushes;
        public object SelectedItem { get => _opArgs.item; set => _opArgs.item = value; }
        public Vector3Int Origin => Grid.Bounds.position;
        public bool IsInitialized => Grid != null;
        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;
        public ScriptableObject Material => material;

        private void Awake()
        {
            if (!defaultMaps[0]) throw new ArgumentNullException(nameof(defaultMaps));
            if (!defaultMaps[0].layoutGrid) throw new ArgumentNullException(nameof(Tilemap.layoutGrid));
            if (!tools) throw new ArgumentNullException(nameof(tools));
            _opArgs.bag = buildBag as IBuildBag ?? throw new ArgumentNullException(nameof(buildBag));
            _opArgs.builder = this;
        }

        private void Start()
        {
            _opArgs.grid = string.IsNullOrEmpty(filePath)
                ? new(defaultMaps)
                : new GridData(serializer, filePath);
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

            if (command == null)
            {
                if (error != null) return Fail(error);
                LastError = null;
                Changed?.Invoke();
                return true;
            }

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
            var cell = defaultMaps[0].WorldToCell(world);
            return new Vector2Int(cell.x - Grid.Bounds.x, cell.y - Grid.Bounds.y);
        }

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
            Grid?.Save(serializer, filePath);
        }

        public void LoadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("no filePath");
            _opArgs.grid = new(serializer, filePath = path);
        }

        private bool Fail(string error)
        {
            LastError = error;
            return false;
        }
    }
}