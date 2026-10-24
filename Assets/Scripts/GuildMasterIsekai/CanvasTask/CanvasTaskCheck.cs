#if UNITY_EDITOR
using System;
using System.Collections.ObjectModel;
using System.Linq;
using GuildMasterIsekai.Model;
using TileGrid.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    internal static class CanvasTaskCheck
    {
        [MenuItem("Tools/Canvas Task/Check Game Canvas")]
        private static void Check()
        {
            CheckList();
            CheckViews();
            var canvas = new GameCanvasUi();
            foreach (var name in new[] { "guild", "clients", "problems", "quests", "adventurers", "party", "map", "options" })
            {
                Require(canvas.Q<Button>(name + "Button") != null, "Missing navigation: " + name);
                Require(canvas.Q<Button>(name + "Close") != null, "Missing close button: " + name);
            }
            const string prefabPath = "Assets/Scripts/GuildMasterIsekai/CanvasTask/GameCanvasUIToolkit.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Require(prefab && prefab.GetComponent<GameCanvasHost>(), "Prefab must reference GameCanvasHost.");
            Debug.Log("Game canvas checks passed: collection updates, filtering, rebinding, cleanup, model events, layout controls, and prefab host.");
        }

        private static void CheckList()
        {
            var source = new ObservableCollection<string> { "first", "second" };
            var view = new ListView<ProbeView>();
            var count = -1;
            view.CountChanged += value => count = value;
            view.Data = source;
            var list = view.Q<UnityEngine.UIElements.ListView>();
            Require(count == 2 && list.itemsSource.Count == 2, "Initial list binding failed.");
            var row = (ProbeView)list.makeItem();
            list.bindItem(row, 1);
            Require(row.Data == "second", "Rows must bind from the actual items source.");
            source[1] = "replacement";
            Require((string)list.itemsSource[1] == "replacement", "Replacement must refresh the affected item.");
            source.Add("third");
            Require(count == 3 && row.Data == null, "Collection changes must release old row bindings.");
            view.Filter = item => ((string)item).StartsWith("r", StringComparison.Ordinal);
            Require(count == 3 && list.itemsSource.Count == 1, "Filtering must preserve the source count.");
            source[1] = "hidden";
            Require(list.itemsSource.Count == 0, "Replacing a filtered item must update membership.");
            view.Filter = null;
            view.Data = new[] { "new" }.Select(value => value);
            source.Clear();
            Require(count == 1 && list.itemsSource.Count == 1, "Rebinding must release the old collection subscription.");
            list.bindItem(row, 0);
            Require(row.Data == "new", "Plain enumerables must bind correctly.");
            list.unbindItem(row, 0);
            Require(row.Data == null, "Recycled rows must release their data.");
            list.destroyItem(row);
            view.Data = null;
            Require(count == 0 && list.itemsSource.Count == 0, "An unbound list must be empty.");
        }

        private static void CheckViews()
        {
            var first = new Guild();
            var second = new Guild();
            var guild = new GuildView { Data = first };
            first.Gold.Value = 42;
            Require(guild.Q<Label>("gold").text == "Gold: 42", "Gold events must update the header.");
            guild.Data = second;
            guild.Q<Label>("gold").text = "unsubscribed";
            first.Gold.Value = 100;
            Require(guild.Q<Label>("gold").text == "unsubscribed", "Old guild subscriptions must be removed.");
            second.Gold.Value = 7;
            Require(guild.Q<Label>("gold").text == "Gold: 7", "The new guild must remain subscribed.");
            guild.Data = null;

            var member = new Adventurer();
            var party = new PartyView { Data = new[] { member } };
            member.Stats.Power.Value = 9;
            Require(party.Q<Label>("partyTotals").text.Contains("Power: 9"), "Party totals must follow stat changes.");
            party.Data = null;
            member.Stats.Power.Value = 10;
            Require(party.Q<Label>("partyTotals").text == "", "Unbinding must release party stat subscriptions.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private sealed class ProbeView : ViewElement<string>
        {
            public ProbeView() { }
            protected override void Subscribe() { }
            protected override void Unsubscribe() { }
            public override void Refresh() { }
            public override void SetSelected(object selected) { }
        }
    }
}
#endif
