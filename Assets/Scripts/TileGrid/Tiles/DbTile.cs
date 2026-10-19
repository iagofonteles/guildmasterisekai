using Drafts;
using Drafts.Database;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/DbTile")]
    public class DbTile : Tile, ITileEntry
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] internal ScriptableObject item;

        public uint Id => id;
        public ITileSo Item => (ITileSo)item;
        TileBase ITileEntry.Tile => this;

        [ContextMenu("Reroll Id")]
        private void RerollId()
        {
            id = UIntGenerator.Instance.Generate();
        }

        private void Reset()
        {
            flags = TileFlags.None;
        }
    }
}