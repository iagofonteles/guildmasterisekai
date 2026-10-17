using GuildMasterIsekai.Systems;
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
        public System.Func<Vector2Int, int> nearestSlot;
    }
}