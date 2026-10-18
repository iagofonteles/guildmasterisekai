using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
  
    [CreateAssetMenu(menuName = "Tiles/DbTile")]
    public class DbTile : Tile,ITileEntry
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] private DatabaseEntrySo item;

        public uint Id => id;
        public DatabaseEntrySo Item => item;
        TileBase ITileEntry.Tile => this;
        
        [ContextMenu("Reroll Id")]
        private void RerollId()
        {
            do
            {
                id = Game.Id.Generate();
            } while (Game.Tiles.Find(id) != null);
        }
    }
}