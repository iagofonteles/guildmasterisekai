using TileGrid.Tiles;
using TileGrid.Build;
using Drafts.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace TileGrid.UI
{
    public sealed class SlotElement : Button, IViewElement
    {
        private readonly Label _amount;
        private readonly Image _image;
        private readonly Label _placeholder;
        private ISlot _slot;
        private bool _subscribed;
        public VisualElement Element => this;

        public object Data
        {
            get => _slot;
            set {
                Unsubscribe();
                _slot = value as ISlot;
                if (panel != null) Subscribe();
                Refresh();
                RefreshAmount(_slot?.Amount ?? 0);
            }
        }

        public SlotElement(ISlot data)
        {
            AddToClassList("tile-choice");
            _image = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
                pickingMode = PickingMode.Ignore
            };
            _image.AddToClassList("slot-icon");
            Add(_image);
            _placeholder = new Label("?") { pickingMode = PickingMode.Ignore };
            _placeholder.AddToClassList("slot-placeholder");
            Add(_placeholder);

            _amount = new Label { pickingMode = PickingMode.Ignore };
            _amount.AddToClassList("tile-amount");
            Add(_amount);
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                Subscribe();
                Refresh();
                RefreshAmount(_slot?.Amount ?? 0);
            });
            RegisterCallback<DetachFromPanelEvent>(_ => Unsubscribe());

            Data = data;
        }

        private void Subscribe()
        {
            if (_slot == null || _subscribed) return;
            _slot.OnAmountChanged += RefreshAmount;
            _slot.OnItemChanged += Refresh;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed) return;
            _slot.OnAmountChanged -= RefreshAmount;
            _slot.OnItemChanged -= Refresh;
            _subscribed = false;
        }

        private void RefreshAmount(int amount)
        {
            _amount.text = amount > 0 ? "x" + amount : string.Empty;
            _amount.EnableInClassList("hidden", amount < 1);
        }

        private void Refresh<T>(T _) => Refresh();

        public void Refresh()
        {
            var item = _slot?.Item as ITile;
            var icon = item?.Icon;
            tooltip = item?.DisplayName ?? string.Empty;
            _image.sprite = icon;
            _image.tintColor = Color.white;
            _image.EnableInClassList("hidden", !icon);
            _placeholder.EnableInClassList("hidden", icon);
        }

        public void SetSelected(object item)
        {
            var value = _slot?.Item;
            var selected = value != null && Equals(value, item);
            EnableInClassList("selected", selected);
        }
    }
}