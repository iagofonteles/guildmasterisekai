using System.Collections.Generic;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/GuildObject")]
    public class GuildObject : DatabaseEntrySo
    {
        [SerializeField] private TileBase tile;
        [SerializeField] private AutoTileType buildOn;
        [SerializeField] private Vector2[] toppingSlots;

        public int Layer => 1;
        public TileBase Tile => tile;
        public AutoTileType BuildOn => buildOn;
        public IReadOnlyList<Vector2> ToppingSlots => toppingSlots;
    }
}