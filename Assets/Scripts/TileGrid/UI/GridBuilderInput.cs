using System;
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

        private bool _uiGesture;
        internal Vector3 PointerWorld { get; private set; }

        public GridBuilder Builder => builder;
        public BuildLayer Layer => layer;
        public string Status { get; private set; }
        public event Action SelectionChanged;
        public event Action StatusChanged;
        public DatabaseEntrySo SelectedTile => layer switch
        {
            BuildLayer.Base => baseTile,
            BuildLayer.Furniture => furnitureTile,
            BuildLayer.Topping => toppingTile,
            _ => null
        };

        private void Awake()
        {
            if (!viewCamera) viewCamera = Camera.main;
            _pixelBrush = new BuildPixelBrush(builder, this);
            _rectBrush = new BuildRectBrush(builder, this);
        }

        private void OnDisable() => CancelGesture();
        private void OnApplicationFocus(bool focused) { if (!focused) CancelGesture(); }

        public void SetLayer(BuildLayer value)
        {
            layer = value;
            if (action == GridBuildAction.Remove || !Supports(layer, action)) action = GridBuildAction.Place;
            SelectionUpdated();
        }

        public void SetAction(GridBuildAction value)
        {
            if (!Supports(layer, value)) return;
            action = value == GridBuildAction.Remove ? GridBuildAction.Place : value;
            SelectionUpdated();
        }

        public void SetBrush(GridBuildBrush value) { CancelGesture(); brush = value; SelectionUpdated(); }

        public void SelectTile(DatabaseEntrySo tile)
        {
            switch (layer)
            {
                case BuildLayer.Base: baseTile = tile as BaseTileSo; break;
                case BuildLayer.Furniture: furnitureTile = tile as FurnitureTileSo; break;
                case BuildLayer.Topping: toppingTile = tile as ToppingTileSo; break;
            }
            SelectionUpdated();
        }

        public static bool Supports(BuildLayer layer, GridBuildAction action) => action switch
        {
            GridBuildAction.Paint => layer == BuildLayer.Base,
            GridBuildAction.Move => layer == BuildLayer.Furniture,
            _ => true
        };

        private void SelectionUpdated()
        {
            CancelGesture();
            Status = null;
            SelectionChanged?.Invoke();
        }

        public void CancelGesture()
        {
            _pixelBrush?.CancelGesture();
            _rectBrush?.CancelGesture();
        }

        public void Undo() { CancelGesture(); Complete(builder && builder.IsInitialized && builder.Undo()); }
        public void Redo() { CancelGesture(); Complete(builder && builder.IsInitialized && builder.Redo()); }

        private void Update()
        {
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

        // Kept separate from device polling so gesture/UI isolation can be validated deterministically.
        internal void ProcessPointer(Vector2 screen, bool pressed, bool held, bool released, bool cancel, bool overUI,
            bool rightHeld = false, bool rightReleased = false)
        {
            if (pressed || cancel) _uiGesture = overUI;
            if (_uiGesture || overUI)
            {
                CancelGesture();
                if (!held && !rightHeld) _uiGesture = false;
                return;
            }
            if (!TryGetPointer(screen, out var cell, out var world))
            {
                CancelGesture();
                return;
            }
            PointerWorld = world;
            var activeBrush = CurrentBrush;
            if (activeBrush == null) return;
            if (pressed) activeBrush.OnMouseDown(cell, MouseButton.Left);
            if (cancel) activeBrush.OnMouseDown(cell, MouseButton.Right);
            if (held) activeBrush.OnMouseDrag(cell, MouseButton.Left);
            if (rightHeld) activeBrush.OnMouseDrag(cell, MouseButton.Right);
            if (released) activeBrush.OnMouseUp(cell, MouseButton.Left);
            if (rightReleased) activeBrush.OnMouseUp(cell, MouseButton.Right);
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            foreach (var key in keyboard.allKeys)
            {
                if (key.wasPressedThisFrame) activeBrush.OnKeyDown(cell, key.keyCode);
                if (key.wasReleasedThisFrame) activeBrush.OnKeyUp(cell, key.keyCode);
            }
        }

        internal void SetStatus(string value)
        {
            Status = value;
            StatusChanged?.Invoke();
        }

        internal void Complete(bool success)
        {
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
