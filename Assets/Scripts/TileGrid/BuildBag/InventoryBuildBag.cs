using System;
using System.Collections.Generic;
using Drafts.Inventory;
using GuildMasterIsekai.Systems;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Adapter for item-based costs in a caller-owned Drafts inventory.</summary>
    public sealed class InventoryBuildBag : IBuildBag
    {
        private readonly ListInventory<DatabaseEntrySo> _inventory;
        private readonly DatabaseEntrySo _buildingMaterial;
        private bool _applying;

        public InventoryBuildBag(ListInventory<DatabaseEntrySo> inventory, DatabaseEntrySo buildingMaterial)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _buildingMaterial = buildingMaterial
                ? buildingMaterial
                : throw new ArgumentNullException(nameof(buildingMaterial));
        }

        public int GetBuildingMaterials() => _inventory.Count(_buildingMaterial);

        public IEnumerable<BaseTileSo> GetUnlockedTiles()
        {
            foreach (var slot in _inventory)
                if (slot.Item is BaseTileSo so)
                    yield return so;
        }

        public IEnumerable<(T, int)> GetAvailableItems<T>()
        {
            foreach (var slot in _inventory)
                if (slot.Item is T item)
                    yield return (item, slot.Amount);
        }

        public bool CanPay(OperationCost cost) => CanApply(cost, false);
        public bool CanRefund(OperationCost cost) => CanApply(cost, true);
        public void Pay(OperationCost cost) => Apply(cost, false);
        public void Refund(OperationCost cost) => Apply(cost, true);

        private bool CanApply(OperationCost cost, bool refund)
        {
            if (_applying || cost == null) return false;

            foreach (var pair in cost.Items)
            {
                if (pair.Key is not DatabaseEntrySo item || !item) return false;
                var delta = refund ? pair.Value : -(long)pair.Value;
                var count = _inventory.Count(item) + delta;
                if (count < 0 || count > int.MaxValue || delta > int.MaxValue || delta < -int.MaxValue ||
                    (delta > 0 && _inventory.Fits(item) < delta)) return false;
            }

            return true;
        }

        private void Apply(OperationCost cost, bool refund)
        {
            if (!CanApply(cost, refund))
                throw new InvalidOperationException("Insufficient items or inventory capacity.");
            _applying = true;

            try
            {
                foreach (var pair in cost.Items)
                {
                    var item = (DatabaseEntrySo)pair.Key;
                    var delta = refund ? pair.Value : checked(-pair.Value);
                    if (delta > 0) _inventory.Add(item, delta);
                    else if (delta < 0) _inventory.Remove(item, -delta);
                }
            }
            finally
            {
                _applying = false;
            }
        }
    }
}