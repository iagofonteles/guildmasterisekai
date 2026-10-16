using System;
using System.IO;
using Drafts;
using GuildMasterIsekai.Systems;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask.Editor
{
    public static class CanvasTaskValidation
    {
        [InitializeOnLoadMethod]
        private static void ScheduleValidation()
        {
            EditorApplication.delayCall += Validate;
        }

        [MenuItem("Tools/Canvas Task/Validate UI Toolkit conversion")]
        public static void Validate()
        {
            const string reportPath = "Temp/CanvasTaskValidation/editor-validation.txt";
            try
            {
                var tree = Resources.Load<VisualTreeAsset>("CanvasTask/GameCanvas");
                Require(tree != null, "UXML imported");
                Require(Resources.Load<ThemeStyleSheet>("CanvasTask/GameCanvasTheme") != null, "Theme imported");
                var root = tree.CloneTree();
                foreach (var name in new[] { "guild", "clients", "problems", "quests", "adventurers", "party", "map", "options" })
                {
                    Require(root.Q(name + "Panel") != null, name + " panel");
                    Require(root.Q<Button>(name + "Button") != null, name + " navigation");
                    Require(root.Q<Button>(name + "Close") != null, name + " close");
                }
                foreach (var name in new[] { "clientsList", "problemsList", "questsList", "questRequestsList",
                    "adventurersList", "freelancersList", "partyList", "solutionsList" })
                    Require(root.Q<ScrollView>(name) != null, name);

                var watcher = new Watcher<int>();
                var changes = 0;
                var binding = new UiBinding<int>(new VisualElement(), (value, scope) =>
                    scope.Watch(watcher, () => changes++));
                binding.Bind(1);
                watcher.Value++;
                binding.Bind(2);
                watcher.Value++;
                Require(changes == 2, "Rebind removes old listeners");
                binding.Dispose();
                binding.Dispose();
                watcher.Value++;
                Require(changes == 2, "Dispose removes listeners and is idempotent");

                var list = new ObservableList<int>();
                var host = new VisualElement();
                var count = -1;
                var rowDisposals = 0;
                var collection = new UiCollectionBinding<int>(host, value =>
                    new UiBinding<int>(new Label(), (item, scope) =>
                    {
                        ((Label)scope.Root).text = item.ToString();
                        scope.OnUnbind(() => rowDisposals++);
                    }));
                collection.CountChanged += value => count = value;
                collection.Bind(list);
                Require(count == 0 && host.Q("emptyState") != null, "Empty state");
                list.Add(1);
                Require(count == 1 && host.childCount == 1, "Add notification");
                list[0] = 2;
                Require(((Label)host[0]).text == "2" && rowDisposals == 1, "Replace and row disposal");
                collection.Filter = value => value > 2;
                collection.Refresh();
                Require(count == 1 && host.Q("emptyState") != null, "Filtering preserves source count");
                var replacement = new ObservableList<int>(new[] { 3 });
                collection.Bind(replacement);
                list.Clear();
                Require(count == 1 && ((Label)host[0]).text == "3", "Rebind detaches old collection");
                replacement.Clear();
                Require(count == 0, "Reset notification");
                collection.Dispose();
                collection.Dispose();
                replacement.Add(4);
                Require(host.childCount == 0, "Disposed collection remains detached");

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Script/CanvasTask/GameCanvasUIToolkit.prefab");
                Require(prefab != null && prefab.GetComponent<GameCanvasUi>() != null, "Prefab controller reference");
                Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
                File.WriteAllText(reportPath, "PASS: UXML/theme import, named controls, prefab reference, " +
                    "binding rebind/disposal, collection add/replace/reset/filter/source replacement.\n" +
                    "Visual layout and gameplay still require Play mode review.\n");
                Debug.Log("Canvas Task validation passed. See " + reportPath);
            }
            catch (Exception exception)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
                File.WriteAllText(reportPath, "FAIL: " + exception);
                Debug.LogException(exception);
            }
        }

        private static void Require(bool condition, string check)
        {
            if (!condition) throw new InvalidOperationException("Canvas Task validation failed: " + check);
        }
    }
}
