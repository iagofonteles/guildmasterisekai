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
        public bool IsPixel => true;
        
        public void CancelGesture() { }

        public void OnMouseDown(IBuildTool tool, Vector2Int pos, MouseButton button)
        {
            Builder.TryUseTool(new(pos, Vector2Int.one), tool);
        }

        public void OnMouseDrag(IBuildTool tool, Vector2Int pos, MouseButton button)
        {
            Builder.TryUseTool(new(pos, Vector2Int.one), tool);
        }

        public void OnMouseUp(IBuildTool tool, Vector2Int pos, MouseButton button) { }
        public void OnKeyDown(Vector2Int pos, Key key) { }
        public void OnKeyUp(Vector2Int pos, Key key) { }
    }
}