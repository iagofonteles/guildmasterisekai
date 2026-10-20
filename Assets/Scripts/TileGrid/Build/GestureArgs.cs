using System;
using TileGrid.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace TileGrid.Build
{
    public sealed class GestureArgs
    {
        public GridBuilder builder;
        public GridData grid;
        public IBuildBag bag;
        public IBuildBrush brush;
        public IBuildTool tool;
        public int layer;
        public object item;
        public Color color;
        public RectInt area;
        public MouseButton button;
        public string lastError;

        public void Validate2()
        {
            if (!builder) throw new ArgumentNullException(nameof(builder));
            if (grid == null) throw new NullReferenceException(nameof(grid));
            if (bag == null) throw new NullReferenceException(nameof(bag));
            if (!builder.Material) throw new NullReferenceException(nameof(builder.Material));
        }

        public IBuildCommand Error(string message)
        {
            lastError = message;
            return null;
        }
    }
}