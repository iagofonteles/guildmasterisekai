using Drafts;
using Drafts.Database;
using UnityEngine;

namespace TileGrid.Tiles
{
    [CreateAssetMenu(menuName = "Tiles/ToppingTile")]
    public class DecorationSo : DatabaseItem, IDecorationEntry
    {
        [SerializeField, Prefab] private GameObject prefab;
        public GameObject Prefab => prefab;
        public BuildLayerMask LayerMask => 0;
    }
}