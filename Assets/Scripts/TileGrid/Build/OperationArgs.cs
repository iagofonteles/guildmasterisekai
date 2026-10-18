using System;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class OperationArgs
    {
        public GridData grid;
        public GridBuilder builder;
        public IBuildBag bag;
        public RectInt area;
        public object item;
        public int slot = -1;
        public Vector2Int delta;
        public Func<Vector2Int, int> nearestSlot;

        public void Validate()
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (grid == null) throw new NullReferenceException(nameof(grid));
            if (bag == null) throw new NullReferenceException(nameof(bag));
            if (!builder.Material) throw new NullReferenceException(nameof(builder.Material));
        }
    }
}