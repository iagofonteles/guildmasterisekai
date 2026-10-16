using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/GuildTopping")]
    public class GuildTopping : DatabaseEntrySo
    {
        [SerializeField] private GameObject prefab;

        public int Layer => 2;
        public GameObject Prefab => prefab;
    }
}