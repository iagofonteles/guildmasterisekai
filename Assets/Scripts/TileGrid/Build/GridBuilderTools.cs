using System;
using System.Collections.Generic;
using Drafts;
using Drafts.Localization;
using UnityEngine;
using UnityEngine.Localization;

namespace GuildMasterIsekai.TileGrid
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
        [SerializeField] private LayerToolConfig[] tools;

        public BuildLayer Layer => layer;
        public LayerToolConfig[] Tools => tools;
    }

    [Serializable]
    public class LayerToolConfig
    {
        [SerializeField, Loc] private LocalizedString str;
        [SerializeReference, TypeInstance(true)] private IBuildTool main;
        [SerializeReference, TypeInstance(true)] private IBuildTool alt;

        public string DisplayName => str.GetLocalizedString();
        public IBuildTool Main => main;
        public IBuildTool Alt => alt;
    }
}