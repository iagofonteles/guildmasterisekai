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
        public bool IsPixel => false;

        public void CancelGesture() => _cancel = true;

        public void OnMouseDown(IBuildTool tool, Vector2Int pos, MouseButton button)
        {
            _cancel = false;
            _first = _last = pos;
        }

        public void OnMouseDrag(IBuildTool tool, Vector2Int pos, MouseButton button) => _last = pos;

        public void OnMouseUp(IBuildTool tool, Vector2Int pos, MouseButton button)
        {
            if (_cancel) return;
            _last = pos;

            var min = Vector2Int.Min(_first, _last);
            var max = Vector2Int.Max(_first, _last);
            var area = new RectInt(min, max - min + Vector2Int.one);
            Builder.TryUseTool(area, tool);
        }

        public void OnKeyDown(Vector2Int pos, Key key)
        {
            if (key == Key.Escape) _cancel = true;
        }

        public void OnKeyUp(Vector2Int pos, Key key) { }
    }
}