using System;
using TileGrid.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace TileGrid.Build
{
    public sealed class BuildArgs
    {
        public GridBuilder builder;
        public GridData grid;
        public IBuildBag bag;
        public RectInt area;
        public MouseButton button;
        public bool alt;
        public bool cancel;
        public readonly Watcher<int> layer = new();
        public readonly Watcher<IBuildBrush> brush = new();
        public readonly Watcher<IBuildTool> tool = new();
        public readonly Watcher<object> item = new();
        public readonly Watcher<BuildColor> color = new();
        public readonly Watcher<string> lastError = new();

        public IBuildCommand Error(string message)
        {
            lastError.Value = message;
            return null;
        }
    }
}