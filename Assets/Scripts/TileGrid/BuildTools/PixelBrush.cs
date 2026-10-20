using System;
using TileGrid.Build;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TileGrid.BuildTools
{
    [Serializable]
    public sealed class PixelBrush : IBuildBrush
    {
        [SerializeField] private Sprite icon;

        public Sprite Icon => icon;
        public bool HasDrag => true;

        public void CancelGesture() { }

        public void OnMouseDown(GestureArgs e, Vector2Int pos)
        {
            e.area = new(pos, Vector2Int.one);
            e.builder.TryUseTool(e);
        }

        public void OnMouseDrag(GestureArgs e, Vector2Int pos)
        {
            e.area = new(pos, Vector2Int.one);
            e.builder.TryUseTool(e);
        }

        public void OnMouseUp(GestureArgs e, Vector2Int pos) { }
        public void OnKeyDown(Vector2Int pos, Key key) { }
        public void OnKeyUp(Vector2Int pos, Key key) { }
    }
}