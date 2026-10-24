using Drafts;
using Drafts.UIElements;
using GuildMasterIsekai.Model;
using GuildMasterIsekai.Systems;
using TileGrid.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.Interactables
{
    public class QuestBoard : IInteractable
    {
        void IInteractable.__setTransform(Transform t) => Transform = t;
        public Transform Transform { get; private set; }

        public void BindUI(PanelRenderer panel, VisualElement root)
            => root.Q<QuestBoardUI>().Data = Game.SaveMngr.Get<GuildHall>();
    }

    [UxmlElement]
    public partial class QuestBoardUI : ViewElement<GuildHall>
    {
        private readonly ListView<QuestStripUI> _list = new();

        public QuestBoardUI()
        {
            AddToClassList("quest-board");
            Add(_list);
        }

        protected override void Subscribe() { }
        protected override void Unsubscribe() { }
        public override void Refresh() => _list.Data = Data.QuestBoard;
        public override void SetSelected(object selected) => _list.SetSelected(selected);
    }
}