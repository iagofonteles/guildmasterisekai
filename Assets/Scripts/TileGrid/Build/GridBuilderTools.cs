using System;
using System.Collections.Generic;
using Drafts;
using Drafts.Localization;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Localization;

namespace TileGrid.Build
{
    /// <summary>Runtime building API. Input controllers call the brush methods in cell coordinates.</summary>
    public class GridBuilderTools : ScriptableObject
    {
        [SerializeField] private LayerConfig[] layers;
        [SerializeReference, TypeInstance] private IBuildBrush[] bruhes;
        [SerializeField] private ToolConfig[] tools;

        public IReadOnlyList<LayerConfig> Layers => layers;
        public IReadOnlyList<IBuildBrush> Brushes => bruhes;
        public ToolConfig[] Tools => tools;
    }

    [Serializable]
    public class LayerConfig
    {
        [SerializeField, Loc] private LocalizedString str;
        [SerializeField] private Sprite icon;

        public string DisplayName => str.GetLocalizedString();
        public Sprite Icon => icon;
    }

    [Serializable]
    public class ToolConfig
    {
        [SerializeField, Loc] private LocalizedString str;
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite altIcon;
        [SerializeReference, TypeInstance(true)] private IBuildTool left;
        [SerializeReference, TypeInstance(true)] private IBuildTool right;
        [SerializeReference, TypeInstance(true)] private IBuildTool altTool;

        public BuildLayerMask Layers => left.LayerMask;
        public string DisplayName => str.GetLocalizedString();
        public Sprite Icon => icon;
        public Sprite AltIcon => altIcon;
        public IBuildTool Left => left;
        public IBuildTool Right => right;
        public IBuildTool AltTool => altTool;
    }
}