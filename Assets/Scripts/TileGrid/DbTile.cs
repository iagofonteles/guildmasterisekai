using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/DbTile")]
    public class DbTile : Tile
    {
        [SerializeField] private ScriptableObject guildTile;
        public ScriptableObject GuildTile => guildTile;
    }
}