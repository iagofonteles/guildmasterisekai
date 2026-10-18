using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable]
    public sealed class BuildPixelBrush : IBuildBrush
    {
        [SerializeField] private Sprite icon;

        public GridBuilder Builder { get; set; }
        public Sprite Icon => icon;

        private void Place(Vector2Int pos, IBuildTool tool)
        {
            Builder.TryUseTool(new(pos, Vector2Int.one), tool);
        }

        public void CancelGesture() { }

        public void OnMouseDown(Vector2Int pos, MouseButton button)
        {
            OnMouseDrag(pos, button);
        }

        public void OnMouseDrag(Vector2Int pos, MouseButton button)
        {
            if (button == MouseButton.Left)
                Place(pos, Builder.CurrentTool.Main);
            if (button == MouseButton.Right)
                Place(pos, Builder.CurrentTool.Alt);
        }

        public void OnMouseUp(Vector2Int pos, MouseButton button) { }
        public void OnKeyDown(Vector2Int pos, Key key) { }
        public void OnKeyUp(Vector2Int pos, Key key) { }
    }
}