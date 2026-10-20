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
        public Watcher<IBuildBrush> brush = new();
        public Watcher<IBuildTool> tool = new();
        public Watcher<int> layer = new();
        public Watcher<object> item;
        public Watcher<Color> color;
        public RectInt area;
        public MouseButton button;
        public Watcher<string> lastError;

        public void Validate2()
        {
            if (!builder) throw new ArgumentNullException(nameof(builder));
            if (grid == null) throw new NullReferenceException(nameof(grid));
            if (bag == null) throw new NullReferenceException(nameof(bag));
            if (!builder.Material) throw new NullReferenceException(nameof(builder.Material));
        }

        public IBuildCommand Error(string message)
        {
            lastError.Value = message;
            return null;
        }
    }
}