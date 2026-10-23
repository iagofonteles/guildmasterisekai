using TileGrid.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.Interactables
{
    public class FrontDoor : IInteractable
    {
        void IInteractable.__setTransform(Transform t) => Transform = t;
        public Transform Transform { get; private set; }
        public void SubscribeUI(PanelRenderer ui)
        {
            //TODO ui.FrontDoorUI.Data = this;
        }
    }

    // show open guild/closed guild status with green/red icon and separate button to open/close
    // create 32x32 sprites for icons
    public class FrontDoorUI : ViewElement<FrontDoor>
    {
        protected override void Subscribe()
        {
            throw new System.NotImplementedException();
        }

        protected override void Unsubscribe()
        {
            throw new System.NotImplementedException();
        }

        public override void Refresh()
        {
            throw new System.NotImplementedException();
        }

        public override void SetSelected(object selected) { }
    }
}