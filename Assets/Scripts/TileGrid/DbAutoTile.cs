using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/DbAutoTile")]
    public class DbAutoTile : AutoTile
    {
        [SerializeField] private GuildTile guildTile;
        [SerializeField] private AutoTileType type;

        public GuildTile GuildTile => guildTile;
        public AutoTileType Type => type;
        public void __setType(AutoTileType t) => type = t;
    }
}