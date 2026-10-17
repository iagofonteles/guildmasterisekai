using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/DbAutoTile")]
    public class DbAutoTile : AutoTile, ITileEntry
    {
        [SerializeField, ReadOnly] private uint id;
        [SerializeField] private BaseTileSo item;
        [SerializeField] private AutoTileType type;

        public uint Id => id;
        public BaseTileSo Item => item;
        public AutoTileType Type => type;

        public void __setup(BaseTileSo i, AutoTileType t)
        {
            item = i;
            type = t;
        }

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