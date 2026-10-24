#if UNITY_EDITOR
using System;
using System.Reflection;
using Drafts;
using GuildMasterIsekai.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.Interactables
{
    internal static class QuestBoardCheck
    {
        [MenuItem("Tools/Guild Master/Check Quest Board Reattach")]
        private static void Check()
        {
            var hall = new GuildHall();
            hall.QuestBoard.Add(new Quest());
            hall.QuestBoard.Add(new Quest());
            var view = new QuestBoardUI { Data = hall };
            var list = view.Q<ListView>();
            for (var i = 0; i < 3; i++)
            {
                using (var detach = DetachFromPanelEvent.GetPooled())
                {
                    detach.target = view;
                    view.SendEvent(detach);
                }
                Require(list.itemsSource == null, "Detach must release the list source.");
                using (var attach = AttachToPanelEvent.GetPooled())
                {
                    attach.target = view;
                    view.SendEvent(attach);
                }
                Require(ReferenceEquals(list.itemsSource, hall.QuestBoard) && list.itemsSource.Count == 2,
                    "Reattach must restore the two quests.");
            }
            var handlers = (Delegate)typeof(ObservableList<Quest>)
                .GetField("OnChanged", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(hall.QuestBoard);
            Require(handlers.GetInvocationList().Length == 1, "Reattachment must not duplicate subscriptions.");
            view.Data = null;
            Require(list.itemsSource == null, "Unbinding must release the list source.");
            Debug.Log("Quest board detach/reattach checks passed.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
#endif
