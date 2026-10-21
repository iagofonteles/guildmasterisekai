using System;
using System.Collections.Generic;
using Drafts;
using TileGrid.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileGrid.Build
{
    public sealed class GridBuilder : MonoBehaviour
    {
        [SerializeReference, TypeInstance] private IGridSerializer serializer;
        [SerializeField] private Tilemap[] defaultMaps;
        [SerializeField] private MonoBehaviour buildBag;
        [SerializeField] private ScriptableObject material;
        [SerializeField, Min(1)] private int historyLimit = 64;
        [SerializeField] private string filePath;
        private readonly GestureArgs _gesture = new();

        private readonly List<IBuildCommand> _undo = new();
        private readonly List<IBuildCommand> _redo = new();

        public event Action Changed;

        public GestureArgs Gesture => _gesture;
        public GridData Grid => _gesture.grid;
        public IBuildBag Bag => _gesture.bag;
        public bool IsInitialized => Gesture.grid != null;
        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;
        public ScriptableObject Material => material;

        private void Awake()
        {
            if (!defaultMaps[0]) throw new ArgumentNullException(nameof(defaultMaps));
            if (!defaultMaps[0].layoutGrid) throw new ArgumentNullException(nameof(Tilemap.layoutGrid));
            Gesture.bag = buildBag as IBuildBag ?? throw new ArgumentNullException(nameof(buildBag));
            Gesture.builder = this;
        }

        private void Start()
        {
            Gesture.grid = string.IsNullOrEmpty(filePath)
                ? new(defaultMaps)
                : new GridData(serializer, filePath);
        }

        /// <summary>Runs a tool through inventory checks, atomic commit, rendering and undo history.</summary>
        public bool TryUseTool(GestureArgs e)
        {
            if (e.tool == null) throw new ArgumentNullException(nameof(e.tool));
            e.lastError.Value = null;
            var command = e.tool.Value.CreateCommand(e);

            if (command == null)
            {
                if (e.lastError.Value != null)
                    return Fail(e.lastError.Value);
                e.lastError.Value = null;
                Changed?.Invoke();
                return true;
            }

            if (!command.Do()) return Fail("The command could not be applied.");
            _redo.Clear();
            _undo.Add(command);
            if (_undo.Count > Mathf.Max(1, historyLimit)) _undo.RemoveAt(0);
            e.lastError.Value = null;
            Changed?.Invoke();

            return true;
        }

        public void Undo() => Restore(_undo, _redo, true);
        public void Redo() => Restore(_redo, _undo, false);

        private bool Restore(List<IBuildCommand> from, List<IBuildCommand> to, bool reverse)
        {
            if (from.Count == 0) return Fail("No command is available.");

            var command = from[^1];
            var success = reverse ? command.Undo() : command.Do();
            if (!success) return Fail("The command could not be restored.");
            from.RemoveAt(from.Count - 1);
            to.Add(command);
            _gesture.lastError.Value = null;

            Changed?.Invoke();
            return true;
        }

        public void Clear()
        {
            _undo.Clear();
            _redo.Clear();
            _gesture.lastError.Value = null;
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
            Gesture.grid = new(serializer, filePath = path);
            Clear();
        }

        private bool Fail(string error)
        {
            _gesture.lastError.Value = error;
            return false;
        }
    }
}