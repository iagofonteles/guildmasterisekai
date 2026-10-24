using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GuildMasterIsekai.CanvasTask
{
    [DisallowMultipleComponent]
    public sealed class GameCanvasHost : MonoBehaviour
    {
        private UIDocument _document;
        private PanelSettings _settings;
        private GameCanvasUi _canvas;
        private bool _started;

        private void Awake()
        {
            var theme = Resources.Load<ThemeStyleSheet>("CanvasTask/GameCanvasTheme");
            if (!theme) throw new InvalidOperationException("GameCanvas runtime theme is missing.");
            _settings = ScriptableObject.CreateInstance<PanelSettings>();
            _settings.themeStyleSheet = theme;
            _settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            _settings.referenceResolution = new Vector2Int(1920, 1080);
            _settings.sortingOrder = 50;
            _document = gameObject.AddComponent<UIDocument>();
            _document.enabled = false;
            _document.panelSettings = _settings;
        }

        private void OnEnable()
        {
            _document.enabled = true;
            _canvas = new GameCanvasUi();
            _document.rootVisualElement.Add(_canvas);
            if (_started) _canvas.Rebind();
        }

        private void Start()
        {
            _started = true;
            _canvas.Rebind();
        }

        private void Update() => _canvas.Tick();

        private void OnDisable()
        {
            _canvas.Data = null;
            _canvas.RemoveFromHierarchy();
            _document.enabled = false;
        }

        private void OnDestroy() => Destroy(_settings);
    }
}
