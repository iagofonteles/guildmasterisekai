using System;
using System.Collections.Generic;
using Drafts;
using Drafts.Localization;
using UnityEngine;
using UnityEngine.Localization;

namespace TileGrid.Build
{
    /// <summary>Runtime building API. Input controllers call the brush methods in cell coordinates.</summary>
    public class GridBuilderTools : ScriptableObject
    {
        [SerializeReference, TypeInstance] private IBuildBrush[] bruhes;
        [SerializeField] private LayerToolsConfig[] layerTools;

        public IReadOnlyList<IBuildBrush> Bushes => bruhes;
        public LayerToolsConfig[] LayerTools => layerTools;
    }

    [Serializable]
    public class LayerToolsConfig
    {
        [SerializeField] private BuildLayer layer;
        [SerializeReference, TypeInstance(true)] private IBuildTool altTool;
        [SerializeField] private LayerToolConfig[] tools;

        public BuildLayer Layer => layer;
        public IBuildTool AltTool => altTool;
        public LayerToolConfig[] Tools => tools;
    }

    [Serializable]
    public class LayerToolConfig
    {
        [SerializeField, Loc] private LocalizedString str;
        [SerializeField] private Sprite icon;
        [SerializeReference, TypeInstance(true)] private IBuildTool left;
        [SerializeReference, TypeInstance(true)] private IBuildTool right;

        public string DisplayName => str.GetLocalizedString();
        public Sprite Icon => icon;
        public IBuildTool Left => left;
        public IBuildTool Right => right;
    }
}