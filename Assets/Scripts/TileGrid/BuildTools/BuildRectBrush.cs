using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable]
    public sealed class BuildRectBrush : IBuildBrush
    {
        [SerializeField] private Sprite icon;
        private Vector2Int _first, _last;
        private bool _cancel;

        public GridBuilder Builder { get; set; }
        public Sprite Icon => icon;

        public void CancelGesture() { }

        public void OnMouseDown(Vector2Int pos, MouseButton button)
        {
            _cancel = false;
            _first = pos;
        }

        public void OnMouseDrag(Vector2Int pos, MouseButton button) { }

        public void OnMouseUp(Vector2Int pos, MouseButton button)
        {
            if (_cancel) return;

            var min = Vector2Int.Min(_first, _last);
            var max = Vector2Int.Max(_first, _last);
            var area = new RectInt(min, max - min + Vector2Int.one);
            var curr = Builder.CurrentTool;

            if (button == MouseButton.Left)
                Builder.TryUseTool(area, curr.Main);
            if (button == MouseButton.Right)
                Builder.TryUseTool(area, curr.Alt);
        }

        public void OnKeyDown(Vector2Int pos, Key key)
        {
            if (key == Key.Escape) _cancel = true;
        }

        public void OnKeyUp(Vector2Int pos, Key key) { }
    }
}