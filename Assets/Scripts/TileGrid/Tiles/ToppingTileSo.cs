using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/ToppingTile")]
    public class ToppingTileSo : DatabaseEntrySo
    {
        [SerializeField, Prefab] private GameObject prefab;

        public int Layer => 2;
        public GameObject Prefab => prefab;
    }
}