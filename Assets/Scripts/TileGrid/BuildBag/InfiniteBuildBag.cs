using System.Collections.Generic;
using System.Linq;
using Drafts.Database;
using Drafts.Inventory;
using GuildMasterIsekai.Systems;
using UnityEngine;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Optional scene dependency for testing without an inventory.</summary>
    public sealed class InfiniteBuildBag : MonoBehaviour, IBuildBag
    {
        [SerializeField] private ScriptableObject database;
        public int GetBuildingMaterials() => int.MaxValue;

        private ListInventory<IDbTile> _inventory;
        public ListInventory<Color> colors;

        private IEnumerable<Slot<IDbTile>> FetchItems()
            => ((IDatabase)database).GetAll<IDbTile>().Select(i => new Slot<IDbTile>(i, -1, true));

        public IEnumerable<ISlot> GetItems<T>()
        {
            if (typeof(T) == typeof(Color))
            {
                foreach (var slot in colors)
                    yield return slot;
                yield break;
            }
            
            _inventory ??= new(FetchItems(), _ => int.MaxValue);
            foreach (var slot in _inventory)
                if (slot.Item is T)
                    yield return slot;
        }

        public bool CanPay(OperationCost cost) => true;
        public bool CanRefund(OperationCost cost) => true;
        public void Pay(OperationCost cost) { }
        public void Refund(OperationCost cost) { }
    }
}
