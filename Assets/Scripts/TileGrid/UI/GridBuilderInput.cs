using System;
using System.Collections.Generic;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [DisallowMultipleComponent]
    public sealed class GridBuilderInput : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private GridBuilder builder;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private GridBuilderUI userInterface;
        [SerializeField] private BuildLayer layer;
        [SerializeField] private BaseTileSo baseTile;
        [SerializeField] private FurnitureTileSo furnitureTile;
        [SerializeField] private ToppingTileSo toppingTile;

        private bool _uiGesture, _leftGesture, _rightGesture, _selectionReady;
        private Vector2Int _leftCell, _rightCell, _pointerCell;
        private IBuildBrush _gestureBrush;

        public GridBuilder Builder => builder;
        public BuildLayer Layer => layer;
        public string Status { get; private set; }
        public event Action SelectionChanged;
        public event Action StatusChanged;
        public DatabaseEntrySo SelectedTile => builder && builder.IsInitialized
            ? builder.SelectedItem as DatabaseEntrySo : LocalTile;
        private DatabaseEntrySo LocalTile => layer switch
        {
            BuildLayer.Base => baseTile,
            BuildLayer.Furniture => furnitureTile,
            BuildLayer.Topping => toppingTile,
            _ => null
        };

        public IReadOnlyList<LayerToolConfig> AvailableTools
        {
            get
            {
                if (builder)
                    foreach (var config in builder.Tools)
                        if (config != null && config.Layer == layer)
                            return config.Tools ?? Array.Empty<LayerToolConfig>();
                return Array.Empty<LayerToolConfig>();
            }
        }

        public bool HasLayer(BuildLayer value)
        {
            if (builder)
                foreach (var config in builder.Tools)
                    if (config != null && config.Layer == value) return true;
            return false;
        }

        private void SelectLayerTool()
        {
            var available = AvailableTools;
            foreach (var tool in available)
                if (tool != null && ReferenceEquals(tool, builder.CurrentTool)) return;
            builder.CurrentTool = null;
            foreach (var tool in available)
                if (tool != null && (tool.Main != null || tool.Alt != null))
                {
                    builder.CurrentTool = tool;
                    break;
                }
        }

        private void Awake()
        {
            if (!viewCamera) viewCamera = Camera.main;
        }

        private void OnDisable() => CancelGesture();
        private void OnApplicationFocus(bool focused) { if (!focused) CancelGesture(); }

        private void EnsureSelection()
        {
            if (_selectionReady || !builder || !builder.IsInitialized) return;
            _selectionReady = true;
            if (!HasLayer(layer))
                foreach (var config in builder.Tools)
                    if (config != null) { layer = config.Layer; break; }
            SelectLayerTool();
            if (builder.CurrentBrush == null)
                foreach (var brush in builder.Brushes)
                    if (brush != null) { builder.CurrentBrush = brush; break; }
            if (builder.CurrentBrush != null) builder.CurrentBrush.Builder = builder;
            builder.SelectedItem = LocalTile;
            SelectionChanged?.Invoke();
        }

        public void SetLayer(BuildLayer value)
        {
            if (!HasLayer(value)) return;
            CancelGesture();
            layer = value;
            SelectLayerTool();
            SelectionUpdated();
        }

        public void SelectTool(LayerToolConfig tool)
        {
            if (!builder || tool == null) return;
            var available = false;
            foreach (var candidate in AvailableTools)
                if (ReferenceEquals(candidate, tool)) { available = true; break; }
            if (!available) return;
            CancelGesture();
            builder.CurrentTool = tool;
            SelectionUpdated();
        }

        public void SelectBrush(IBuildBrush brush)
        {
            if (!builder || brush == null) return;
            var available = false;
            foreach (var candidate in builder.Brushes)
                if (ReferenceEquals(candidate, brush)) { available = true; break; }
            if (!available) return;
            CancelGesture();
            brush.Builder = builder;
            builder.CurrentBrush = brush;
            SelectionUpdated();
        }

        public void SelectTile(DatabaseEntrySo tile)
        {
            CancelGesture();
            switch (layer)
            {
                case BuildLayer.Base: baseTile = tile as BaseTileSo; break;
                case BuildLayer.Furniture: furnitureTile = tile as FurnitureTileSo; break;
                case BuildLayer.Topping: toppingTile = tile as ToppingTileSo; break;
            }
            SelectionUpdated();
        }

        private void SelectionUpdated()
        {
            if (builder && builder.IsInitialized) builder.SelectedItem = LocalTile;
            Status = null;
            SelectionChanged?.Invoke();
        }

        public void CancelGesture()
        {
            _gestureBrush?.OnKeyDown(_pointerCell, Key.Escape);
            _leftGesture = _rightGesture = false;
            _gestureBrush = null;
        }

        public void Undo() { CancelGesture(); Complete(builder && builder.IsInitialized && builder.Undo()); }
        public void Redo() { CancelGesture(); Complete(builder && builder.IsInitialized && builder.Redo()); }

        private void Update()
        {
            EnsureSelection();
            var mouse = Mouse.current;
            if (mouse == null || !builder || !builder.IsInitialized)
            {
                CancelGesture();
                return;
            }
            if (!viewCamera) viewCamera = Camera.main;
            var screen = mouse.position.ReadValue();
            ProcessPointer(screen, mouse.leftButton.wasPressedThisFrame, mouse.leftButton.isPressed,
                mouse.leftButton.wasReleasedThisFrame, mouse.rightButton.wasPressedThisFrame,
                userInterface && userInterface.BlocksWorldInput(screen,
                    mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame),
                mouse.rightButton.isPressed, mouse.rightButton.wasReleasedThisFrame);
        }

        internal void ProcessPointer(Vector2 screen, bool pressed, bool held, bool released, bool rightPressed,
            bool overUI, bool rightHeld = false, bool rightReleased = false)
        {
            if (!builder || !builder.IsInitialized) { CancelGesture(); return; }
            if (pressed || rightPressed) _uiGesture = overUI;
            if (_uiGesture || overUI)
            {
                CancelGesture();
                if (!held && !rightHeld) _uiGesture = false;
                return;
            }
            if (!TryGetPointer(screen, out var cell, out _)) { CancelGesture(); return; }
            _pointerCell = cell;
            var activeBrush = builder.CurrentBrush;
            if (activeBrush == null || builder.CurrentTool == null) { CancelGesture(); return; }
            if (_gestureBrush != null && !ReferenceEquals(_gestureBrush, activeBrush)) CancelGesture();
            activeBrush.Builder = builder;
            var keyboard = Keyboard.current;
            if (keyboard != null)
                foreach (var key in keyboard.allKeys)
                {
                    if (key.wasPressedThisFrame) activeBrush.OnKeyDown(cell, key.keyCode);
                    if (key.wasReleasedThisFrame) activeBrush.OnKeyUp(cell, key.keyCode);
                }
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) { CancelGesture(); return; }
            ForwardButton(activeBrush, cell, MouseButton.Left, pressed, held, released,
                ref _leftGesture, ref _leftCell);
            ForwardButton(activeBrush, cell, MouseButton.Right, rightPressed, rightHeld, rightReleased,
                ref _rightGesture, ref _rightCell);
            if (!_leftGesture && !_rightGesture) _gestureBrush = null;
        }

        private void ForwardButton(IBuildBrush brush, Vector2Int cell, MouseButton button,
            bool pressed, bool held, bool released, ref bool gesture, ref Vector2Int last)
        {
            var tool = button == MouseButton.Left ? builder.CurrentTool.Main : builder.CurrentTool.Alt;
            if (tool == null) return;
            if (pressed)
            {
                // A brush receives one button's gesture at a time.
                if (_gestureBrush != null) CancelGesture();
                _gestureBrush = brush;
                gesture = true;
                last = cell;
                brush.OnMouseDown(cell, button);
                Complete(string.IsNullOrEmpty(builder.LastError));
            }
            if (!gesture) return;
            if ((held || released) && cell != last)
            {
                last = cell;
                brush.OnMouseDrag(cell, button);
                Complete(string.IsNullOrEmpty(builder.LastError));
            }
            if (!released) return;
            gesture = false;
            brush.OnMouseUp(cell, button);
            Complete(string.IsNullOrEmpty(builder.LastError));
        }

        private void Complete(bool success)
        {
            if (builder && builder.IsInitialized && SelectedTile != LocalTile)
            {
                switch (layer)
                {
                    case BuildLayer.Base: baseTile = SelectedTile as BaseTileSo; break;
                    case BuildLayer.Furniture: furnitureTile = SelectedTile as FurnitureTileSo; break;
                    case BuildLayer.Topping: toppingTile = SelectedTile as ToppingTileSo; break;
                }
                SelectionChanged?.Invoke();
            }
            Status = success ? null : builder ? builder.LastError ?? "Grid is not ready." : "Assign a GridBuilder.";
            StatusChanged?.Invoke();
        }

        private bool TryGetPointer(Vector2 screen, out Vector2Int cell, out Vector3 world)
        {
            cell = default;
            world = default;
            if (!viewCamera || !builder || !builder.IsInitialized || !viewCamera.pixelRect.Contains(screen)) return false;
            var map = tilemap;
            if (!map) return false;
            var origin = map.CellToWorld(builder.Origin);
            var right = map.CellToWorld(builder.Origin + Vector3Int.right) - origin;
            var up = map.CellToWorld(builder.Origin + Vector3Int.up) - origin;
            var normal = Vector3.Cross(right, up);
            if (normal.sqrMagnitude < 0.000001f) return false;
            var ray = viewCamera.ScreenPointToRay(screen);
            if (!new Plane(normal.normalized, origin).Raycast(ray, out var distance)) return false;
            world = ray.GetPoint(distance);
            cell = builder.WorldToCell(world);
            return true;
        }
    }
}
