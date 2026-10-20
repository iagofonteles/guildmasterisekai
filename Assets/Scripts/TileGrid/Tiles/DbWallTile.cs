using Drafts;
using UnityEngine;

namespace TileGrid.Tiles
{
    [CreateAssetMenu(menuName = "Tiles/WallTile")]
    public class DbWallTile : DbAutoTile, IRoofedTile
    {
        [SerializeField, SO] private DbAutoTile roof;
        public ITileEntry Roof => roof;
    }
}