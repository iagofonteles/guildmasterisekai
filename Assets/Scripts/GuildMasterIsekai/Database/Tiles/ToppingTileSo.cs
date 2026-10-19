using System.Collections.Generic;
using Drafts;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    [CreateAssetMenu(menuName = "Tiles/ToppingTile")]
    public class ToppingTileSo : DatabaseEntrySo , IPrefabTile
    {
        [SerializeField, Prefab] private GameObject prefab;

        public GameObject Prefab => prefab;
    }
}