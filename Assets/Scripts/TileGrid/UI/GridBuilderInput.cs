using System;
using System.Collections;
using TileGrid.Build;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace TileGrid.UI
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public sealed class GridBuilderInput : MonoBehaviour
    {
        [SerializeField] private GridBuilder builder;
        [SerializeField] private SelectionUI selection;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Camera camera;
        private BuildArgs Args => builder.BuildArgs;
        private IBuildBrush Brush => Args.brush.Value;

        private VisualElement _toolbar;
        private Vector2Int _first, _last;
        private ButtonControl _dragging;
        private readonly Plane _plane = new(Vector3.forward, Vector3.zero);

        #region UnityEvents

        private IEnumerator Start()
        {
            enabled = false;
            while (!builder.IsInitialized)
                yield return null;

            Subscribe();
            enabled = true;
        }

        private void OnDestroy() => Unsubscribe();

        private void OnDisable() => CancelDrag();

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) CancelDrag();
        }

        private void Subscribe()
        {
            Args.brush.OnChanged += CancelDrag;
            Args.layer.OnChanged += CancelDrag;
            Args.tool.OnChanged += CancelDrag;
        }

        private void Unsubscribe()
        {
            if (!builder) return;
            Args.brush.OnChanged -= CancelDrag;
            Args.layer.OnChanged -= CancelDrag;
            Args.tool.OnChanged -= CancelDrag;
        }

        #endregion

        private void Update()
        {
            if (!builder.IsInitialized) return;
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;
            if (mouse == null || keyboard == null) return;

            var screen = mouse.position.ReadValue();
            Args.alt = keyboard.altKey.isPressed;
            selection.SetCursor(screen, Args.alt ? Args.tool?.Value.AltIcon : null);

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                CancelDrag();
                return;
            }

            var ray = camera.ScreenPointToRay(screen);
            if (!_plane.Raycast(ray, out var distance)) return;

            var worldPos = ray.GetPoint(distance);
            var gridPos = GetGridPosition(worldPos);

            if (_last != gridPos)
            {
                _last = gridPos;

                if (_dragging != null)
                {
                    Args.area = GetArea(_first, _last);
                    Brush.OnMouseDrag(Args, gridPos);
                }
                else Args.area = new(gridPos, Vector2Int.one);

                selection.UpdateArea();
            }

            if (_dragging?.wasReleasedThisFrame ?? false)
            {
                _dragging = null;
                Brush.OnMouseUp(Args, gridPos);
                Args.area = new(gridPos, Vector2Int.one);
                Args.button = MouseButton.LeftMouse;
                selection.UpdateArea();
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (MousePressed(mouse, ref Args.button, ref _dragging))
            {
                _first = gridPos;
                Args.area = new(gridPos, Vector2Int.one);
                Brush.OnMouseDown(Args, gridPos);
                selection.UpdateArea();
            }
        }

        private static bool MousePressed(Mouse mouse, ref MouseButton button, ref ButtonControl ctrl)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                button = MouseButton.LeftMouse;
                ctrl = mouse.leftButton;
                return true;
            }

            if (mouse.rightButton.wasPressedThisFrame)
            {
                button = MouseButton.RightMouse;
                ctrl = mouse.rightButton;
                return true;
            }

            if (mouse.middleButton.wasPressedThisFrame)
            {
                button = MouseButton.MiddleMouse;
                ctrl = mouse.middleButton;
                return true;
            }

            return false;
        }

        private void CancelDrag<T>(T _) => CancelDrag();

        private void CancelDrag()
        {
            Args.cancel = true;
            _dragging = null;
        }

        private Vector2Int GetGridPosition(Vector3 worldPos)
        {
            var worldCell = tilemap.WorldToCell(worldPos);
            var p = worldCell - tilemap.origin;
            var b = builder.Grid.Indexes;
            var x = Math.Clamp(p.x, b.xMin, b.xMax);
            var y = Math.Clamp(p.y, b.yMin, b.yMax);
            return new(x, y);
        }

        private RectInt GetArea(Vector2Int a, Vector2Int b)
        {
            var bounds = builder.Grid.Indexes;
            var minX = Math.Max(Math.Min(a.x, b.x), bounds.xMin);
            var minY = Math.Max(Math.Min(a.y, b.y), bounds.yMin);
            var maxX = Math.Min(Math.Max(a.x, b.x), bounds.xMax - 1);
            var maxY = Math.Min(Math.Max(a.y, b.y), bounds.yMax - 1);
            var w = Math.Max(0, maxX - minX + 1);
            var h = Math.Max(0, maxY - minY + 1);
            return new RectInt(minX, minY, w, h);
        }
    }
}