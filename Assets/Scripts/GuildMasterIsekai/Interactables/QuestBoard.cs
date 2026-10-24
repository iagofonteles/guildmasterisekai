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
        private readonly ListView<QuestStripUI> _list;

        public QuestBoardUI()
        {
            AddToClassList("quest-board");
            _list = new ()
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                selectionType = SelectionType.None,
                makeItem = () => new QuestStripUI(),
                bindItem = (element, index) => ((QuestStripUI)element).Data = _quests[index],
                unbindItem = (element, _) => ((QuestStripUI)element).Data = null
            };
            Add(_list);
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                if (Data == null) return;
                Subscribe();
                Refresh();
            });
            RegisterCallback<DetachFromPanelEvent>(_ => Unsubscribe());
        }

        protected override void Subscribe()
        {
            _quests = Data.QuestBoard;
            _quests.OnChanged += QuestsChanged;
            _list.itemsSource = _quests;
        }

        protected override void Unsubscribe()
        {
            _quests.OnChanged -= QuestsChanged;
            _list.itemsSource = null;
            _quests = null;
        }

        public override void Refresh()
        {
            _list.RefreshItems();
        }

        private void QuestsChanged(ListEvent action, int index, Quest quest) => _list.RefreshItems();

        public override void SetSelected(object selected) { }
    }
}
