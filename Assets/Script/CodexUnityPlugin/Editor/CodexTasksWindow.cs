using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CodexUnityPlugin.Editor
{
    public sealed class CodexTasksWindow : EditorWindow
    {
        [SerializeField] private string question = "";
        [SerializeField] private string answer = "";
        [SerializeField] private List<Object> references = new List<Object>();
        private string executable;
        private string configuredPath;
        private bool initialized;
        private bool starting;
        private Label status;
        private TextField result;
        private VisualElement inputs;
        private Button startButton, answerButton, taskButton, stopButton;
        private static Task<string> operation;
        private static CancellationTokenSource cancellation;
        private bool Busy => operation != null;
        private string Root => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        private string PreferenceKey => "CodexUnityPlugin.Executable." + Root;

        [MenuItem("Tools/Codex Tasks")]
        public static void Open() => GetWindow<CodexTasksWindow>("Codex Tasks");

        private void OnEnable()
        {
            configuredPath = EditorPrefs.GetString(PreferenceKey, "");
            EditorApplication.update += Poll;
            AssemblyReloadEvents.beforeAssemblyReload += Cancel;
            EditorApplication.quitting += Cancel;
        }

        private void OnDisable()
        {
            Cancel();
            initialized = false;
            EditorApplication.update -= Poll;
            AssemblyReloadEvents.beforeAssemblyReload -= Cancel;
            EditorApplication.quitting -= Cancel;
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Script/CodexUnityPlugin/Editor/CodexTasksWindow.uss");
            if (stylesheet != null) rootVisualElement.styleSheets.Add(stylesheet);
            status = new Label();
            rootVisualElement.Add(status);
            inputs = new VisualElement();
            rootVisualElement.Add(inputs);
            var path = new TextField("Codex executable (optional)") { value = configuredPath };
            path.tooltip = "Absolute codex.exe path. Leave blank to search Unity's PATH.";
            path.RegisterValueChangedCallback(e =>
            {
                configuredPath = e.newValue;
                EditorPrefs.SetString(PreferenceKey, configuredPath);
                initialized = false;
                UpdateControls();
            });
            inputs.Add(path);
            var prompt = new TextField("Question") { multiline = true, value = question };
            prompt.AddToClassList("question");
            prompt.RegisterValueChangedCallback(e => question = e.newValue);
            inputs.Add(prompt);
            var objects = new Foldout { text = "Referenced Unity objects", value = true };
            inputs.Add(objects);
            var rows = new ScrollView();
            rows.AddToClassList("references");
            objects.Add(rows);
            Action rebuild = null;
            rebuild = () =>
            {
                rows.Clear();
                for (var i = 0; i < references.Count; i++)
                {
                    var index = i;
                    var row = new VisualElement();
                    row.AddToClassList("row");
                    var field = new ObjectField { objectType = typeof(Object), allowSceneObjects = true, value = references[i] };
                    field.AddToClassList("object");
                    field.RegisterValueChangedCallback(e => references[index] = e.newValue);
                    row.Add(field);
                    row.Add(new Button(() => { references.RemoveAt(index); rebuild(); }) { text = "Remove" });
                    rows.Add(row);
                }
            };
            rebuild();
            objects.Add(new Button(() => { references.Add(null); rebuild(); }) { text = "Add reference (drop onto object field)" });
            var buttons = new VisualElement();
            buttons.AddToClassList("row");
            rootVisualElement.Add(buttons);
            startButton = new Button(StartCodex) { text = "Start Codex" };
            answerButton = new Button(() => Submit(false)) { text = "Answer Question" };
            taskButton = new Button(() => Submit(true)) { text = "Run Task" };
            stopButton = new Button(() => { if (Busy) Cancel(); else { initialized = false; UpdateControls(); } }) { text = "Stop / Cancel" };
            buttons.Add(startButton); buttons.Add(answerButton); buttons.Add(taskButton); buttons.Add(stopButton);
            var scroll = new ScrollView();
            scroll.AddToClassList("output");
            result = new TextField("Answer / Result") { multiline = true, isReadOnly = true, value = answer };
            scroll.Add(result);
            rootVisualElement.Add(scroll);
            UpdateControls();
        }

        private void StartCodex()
        {
            if (Busy) return;
            starting = true;
            initialized = false;
            var root = Root;
            var configured = configuredPath;
            Begin(token => CodexWorkflow.Initialize(root, configured, token));
        }

        private void Begin(Func<CancellationToken, Task<string>> action)
        {
            cancellation = new CancellationTokenSource();
            var token = cancellation.Token;
            // Unity API calls are performed before this boundary or in Poll on the main thread.
            operation = Task.Run(() => action(token), token);
            SetAnswer(starting ? "Validating Codex and authentication..." : "Codex is working. Cancel to stop the process tree.");
            UpdateControls();
        }

        private void Submit(bool task)
        {
            if (Busy || !initialized) return;
            if (string.IsNullOrWhiteSpace(question)) { SetAnswer("Enter a question or task first."); return; }
            try
            {
                var root = Root;
                var context = DescribeReferences();
                var request = question;
                var command = executable;
                starting = false;
                Begin(async token =>
                {
                    var report = task ? CodexWorkflow.ReserveReport(root, request) :
                        Path.Combine(root, CodexWorkflow.OutputFolder, "QuickAnswer.txt");
                    var relative = Path.GetRelativePath(root, report).Replace('\\', '/');
                    var prompt = (task ? "[TaskFile]\n" : "[QuickAnswer]\n") + request +
                        "\n\nReferenced Unity objects (context only, not permission to edit):\n" + context +
                        (task ? "\nFollow the project-root AGENTS.md. Implement this task, preserve unrelated changes, and do not commit. " +
                         "Return the complete plain-text report for the Editor integration to save at exactly: " + relative +
                         ". Do not write this report yourself. Include title, summary, changes, tests, limitations, and 'Modified assets / files:' " +
                         "with known project-relative added/modified/deleted paths. The integration has no verified before/after snapshot; " +
                         "state that the list is agent-reported and unverified; distinguish existing changes.\n" :
                         "\nRead-only question: do not create, edit, rename or delete any project files. Return only your answer as plain text.\n");
                    try
                    {
                        var response = await new CodexProcess().Run(command,
                            "exec --sandbox " + (task ? "workspace-write" : "read-only") + " --color never -",
                            root, prompt, token).ConfigureAwait(false);
                        var text = response.RequireSuccess().Trim();
                        if (text.Length == 0) throw new IOException("Codex exited successfully but returned an empty response.\n" + response.Error);
                        token.ThrowIfCancellationRequested();
                        if (task) text += "\n\nIntegration note: modified paths above are agent-reported, not independently verified.";
                        File.WriteAllText(report, text, new UTF8Encoding(false));
                        return "Saved " + relative + " (exit " + response.ExitCode + ")\n\n" + File.ReadAllText(report);
                    }
                    catch
                    {
                        if (task) File.WriteAllText(report, "Task failed or was cancelled. Changes may be partial.\n" +
                            "Modified assets / files:\nUnknown: execution did not produce a verified completion report.");
                        throw;
                    }
                });
            }
            catch (Exception e) { SetAnswer(e.Message); }
        }

        private string DescribeReferences()
        {
            var text = new StringBuilder();
            foreach (var item in references)
            {
                if (!item) { text.AppendLine("- Missing or empty reference; cannot resolve."); continue; }
                var asset = AssetDatabase.GetAssetPath(item);
                if (!string.IsNullOrEmpty(asset)) { text.AppendLine("- Asset: " + asset + " (" + item.GetType().Name + ": " + item.name + ")"); continue; }
                var go = item as GameObject;
                if (item is Component component) go = component.gameObject;
                if (go)
                {
                    var hierarchy = new List<string>();
                    for (var current = go.transform; current != null; current = current.parent)
                        hierarchy.Insert(0, current.name + "[" + current.GetSiblingIndex() + "]");
                    text.AppendLine("- Scene: " + (string.IsNullOrEmpty(go.scene.path) ? "UNSAVED" : go.scene.path) +
                        "; hierarchy: " + string.Join("/", hierarchy) + "; type: " + item.GetType().Name +
                        "; global ID: " + GlobalObjectId.GetGlobalObjectIdSlow(item) +
                        (go.scene.isDirty ? "; WARNING: unsaved scene changes are unavailable on disk." : ""));
                }
                else text.AppendLine("- Unresolved transient object: " + item.name + " (" + item.GetType().Name + ")");
            }
            return text.Length == 0 ? "None.\n" : text.ToString();
        }

        private void Poll()
        {
            if (operation == null || !operation.IsCompleted) return;
            try
            {
                var text = operation.GetAwaiter().GetResult();
                if (starting) { executable = text; initialized = true; SetAnswer("Codex ready.\n" + executable); }
                else SetAnswer(text);
            }
            catch (OperationCanceledException) { SetAnswer("Cancelled. Task edits may be partial; inspect the working tree before continuing."); }
            catch (Exception e) { SetAnswer(e.Message + "\nCheck CLI configuration and run codex login in a terminal if authentication is required."); }
            finally
            {
                operation = null;
                cancellation.Dispose(); cancellation = null;
                starting = false;
                // Targeted imports only when safe; normal automatic refresh handles task source edits.
                if (!EditorApplication.isCompiling && !EditorApplication.isUpdating)
                    foreach (var file in Directory.Exists(Path.Combine(Root, CodexWorkflow.OutputFolder)) ?
                                 Directory.GetFiles(Path.Combine(Root, CodexWorkflow.OutputFolder), "*.txt") : Array.Empty<string>())
                        AssetDatabase.ImportAsset(Path.GetRelativePath(Root, file).Replace('\\', '/'));
                UpdateControls();
            }
        }

        private static void Cancel() => cancellation?.Cancel();
        private void SetAnswer(string text) { answer = text; result?.SetValueWithoutNotify(text); }
        private void UpdateControls()
        {
            if (status == null) return;
            status.text = "Codex: " + (Busy ? "Working" : initialized ? "Running (idle)" : "Shutdown");
            inputs.SetEnabled(!Busy);
            startButton.SetEnabled(!Busy && !initialized);
            answerButton.SetEnabled(!Busy && initialized);
            taskButton.SetEnabled(!Busy && initialized);
            stopButton.SetEnabled(Busy || initialized);
        }
    }
}
