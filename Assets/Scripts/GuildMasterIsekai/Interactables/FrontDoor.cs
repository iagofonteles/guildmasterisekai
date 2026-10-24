using GuildMasterIsekai.Model;
using GuildMasterIsekai.Systems;
using TileGrid.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.Interactables
{
    public class FrontDoor : IInteractable
    {
        void IInteractable.__setTransform(Transform t) => Transform = t;
        public Transform Transform { get; private set; }

        public void BindUI(PanelRenderer panel, VisualElement root)
            => root.Q<FrontDoorUI>().Data = Game.SaveMngr.Get<GuildHall>();
    }

    [UxmlElement]
    public partial class FrontDoorUI : ViewElement<GuildHall>
    {
        private readonly Label _status = new();
        private readonly Button _toggle;

        public FrontDoorUI()
        {
            AddToClassList("front-door");
            _status.AddToClassList("door-status");
            Add(_status);
            _toggle = new Button(() => Data.OpenStatus.Value = !Data.OpenStatus.Value);
            Add(_toggle);
            SetEnabled(false);
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                if (Data != null) Refresh();
            });
            RegisterCallback<DetachFromPanelEvent>(_ => Unsubscribe());
        }

        protected override void Subscribe() => Data.OpenStatus.OnChanged += StatusChanged;
        protected override void Unsubscribe() => Data.OpenStatus.OnChanged -= StatusChanged;
        public override void SetSelected(object selected) { }

        public override void Refresh()
        {
            SetEnabled(Data != null);
            if (Data == null) return;
            StatusChanged(Data.OpenStatus.Value);
        }

        private void StatusChanged(bool value)
        {
            _status.text = value ? "Guild open" : "Guild closed";
            _status.EnableInClassList("is-open", value);
            _toggle.text = value ? "Close guild" : "Open guild";
        }
    }
}