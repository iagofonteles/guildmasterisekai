using System;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GuildMasterIsekai.TileGrid
{
    public enum GridBuildAction { Place, Paint, Remove, Move }
    public enum GridBuildBrush { Pixel, Rectangle }

    [DisallowMultipleComponent, RequireComponent(typeof(GridBuilder))]
    public sealed class GridBuilderInput : MonoBehaviour
    {
        [SerializeField] private GridBuilder builder;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private GridBuilderUI userInterface;
        [SerializeField] private BuildLayer layer;
        [SerializeField] private GridBuildAction action;
        [SerializeField] private GridBuildBrush brush;
        [SerializeField] private BaseTileSo baseTile;
        [SerializeField] private FurnitureTileSo furnitureTile;
        [SerializeField] private ToppingTileSo toppingTile;

        private bool _dragging, _uiGesture;
        private Vector2Int _first, _last;
        private RectInt? _moveArea;

        public GridBuilder Builder => builder;
        public BuildLayer Layer => layer;
        public GridBuildAction Action => action;
        public GridBuildBrush Brush => brush;
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
            if (!builder) builder = GetComponent<GridBuilder>();
            if (!userInterface) userInterface = GetComponent<GridBuilderUI>();
            if (!viewCamera) viewCamera = Camera.main;
        }

        private void OnDisable() => CancelGesture();
        private void OnApplicationFocus(bool focused) { if (!focused) CancelGesture(); }

        public void SetLayer(BuildLayer value)
        {
            layer = value;
            if (!Supports(layer, action)) action = GridBuildAction.Place;
            SelectionUpdated();
        }

        public void SetAction(GridBuildAction value)
        {
            if (!Supports(layer, value)) return;
            action = value;
            SelectionUpdated();
        }

        public void SetBrush(GridBuildBrush value) { brush = value; SelectionUpdated(); }

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
            var hadMoveSelection = _moveArea.HasValue;
            _dragging = false;
            _moveArea = null;
            if (hadMoveSelection)
            {
                Status = null;
                StatusChanged?.Invoke();
            }
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
                userInterface && userInterface.BlocksWorldInput(screen, mouse.leftButton.wasPressedThisFrame));
        }

        // Kept separate from device polling so gesture/UI isolation can be validated deterministically.
        internal void ProcessPointer(Vector2 screen, bool pressed, bool held, bool released, bool cancel, bool overUI)
        {
            if (cancel) { CancelGesture(); return; }
            if (pressed) _uiGesture = overUI;
            if (_uiGesture || overUI)
            {
                CancelGesture();
                if (!held) _uiGesture = false;
                return;
            }
            if (!TryGetPointer(screen, out var cell, out var world))
            {
                CancelGesture();
                return;
            }
            if (pressed)
            {
                _dragging = true;
                _first = _last = cell;
                if (UsesPixelStroke()) Apply(GridBuilder.Pixel(cell), cell, world);
            }
            if (!_dragging) return;
            if ((held || released) && UsesPixelStroke() && cell != _last)
            {
                _last = cell;
                Apply(GridBuilder.Pixel(cell), cell, world);
            }
            if (!released) return;
            _dragging = false;
            if (action == GridBuildAction.Move)
            {
                if (brush == GridBuildBrush.Pixel)
                    Complete(builder.TryMove(GridBuilder.Pixel(_first), cell - _first));
                else if (_moveArea.HasValue)
                {
                    var moved = builder.TryMove(_moveArea.Value, cell - _moveArea.Value.min);
                    if (moved) _moveArea = null;
                    Complete(moved);
                }
                else
                {
                    _moveArea = GridBuilder.Rectangle(_first, cell);
                    Status = "Source selected. Click the new bottom-left cell. Right-click cancels.";
                    StatusChanged?.Invoke();
                }
            }
            else if (!UsesPixelStroke()) Apply(GridBuilder.Rectangle(_first, cell), cell, world);
        }

        private bool UsesPixelStroke() => action != GridBuildAction.Move &&
            (brush == GridBuildBrush.Pixel || layer == BuildLayer.Topping && action == GridBuildAction.Place);

        private void Apply(RectInt area, Vector2Int cell, Vector3 world)
        {
            bool success;
            if (action == GridBuildAction.Remove) success = builder.TryRemove(area, layer, world);
            else if (action == GridBuildAction.Paint) success = builder.TryPaint(area, baseTile);
            else if (layer == BuildLayer.Base) success = builder.TryPlaceFloor(area, baseTile);
            else if (layer == BuildLayer.Furniture) success = builder.TryPlaceFurniture(area, furnitureTile);
            else success = builder.TryPlaceTopping(cell, toppingTile);
            Complete(success);
        }

        private void Complete(bool success)
        {
            Status = success ? null : builder ? builder.LastError ?? "Grid is not ready." : "Assign a GridBuilder.";
            StatusChanged?.Invoke();
        }

        private bool TryGetPointer(Vector2 screen, out Vector2Int cell, out Vector3 world)
        {
            cell = default;
            world = default;
            if (!viewCamera || !builder || !builder.IsInitialized || !viewCamera.pixelRect.Contains(screen)) return false;
            var map = builder.BaseMap;
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
