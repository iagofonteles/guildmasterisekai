using System.Collections.Generic;
using System.Linq;
using Drafts.Database;
using Drafts.Inventory;
using TileGrid.Build;
using UnityEngine;

namespace TileGrid.BuildBag
{
    /// <summary>Optional scene dependency for testing without an inventory.</summary>
    public sealed class InfiniteBuildBag : MonoBehaviour, IBuildBag
    {
        [SerializeField] private ScriptableObject database;
        public int GetBuildingMaterials() => int.MaxValue;

        private ListInventory<IDatabaseItem> _inventory;
        public List<BuildColor> colors;

        private IEnumerable<Slot<IDatabaseItem>> FetchItems()
            => ((IDatabase)database).GetAll<IDatabaseItem>().Select(i => new Slot<IDatabaseItem>(i, -1, true));

        public IEnumerable<ISlot> GetSlots<T>()
        {
            if (typeof(T) == typeof(BuildColor))
            {
                foreach (var color in colors)
                    yield return color;
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
