using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/GuildTile")]
    public class GuildTile : DatabaseEntrySo
    {
        [SerializeField] private DbAutoTile floor;
        [SerializeField] private DbAutoTile wall;
        [SerializeField] private DbAutoTile roof;

        public int Layer => 0;
        public TileBase Floor => floor;
        public TileBase Wall => wall;
        public TileBase Roof => roof;

        private void OnValidate()
        {
            if (floor && floor.Type != AutoTileType.Floor)
                floor.__setType(AutoTileType.Floor);
            if (wall && wall.Type != AutoTileType.Wall)
                wall.__setType(AutoTileType.Wall);
            if (roof && roof.Type != AutoTileType.Roof)
                roof.__setType(AutoTileType.Roof);
        }
    }
}