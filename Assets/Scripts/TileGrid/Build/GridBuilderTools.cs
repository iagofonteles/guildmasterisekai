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
        [SerializeReference, TypeInstance] private IBuildBrush[] bruhes;
        [SerializeField] private ToolConfig[] tools;

        public IReadOnlyList<IBuildBrush> Bushes => bruhes;
        public ToolConfig[] Tools => tools;
    }

    [Serializable]
    public class ToolConfig
    {
        [SerializeField] private BuildLayerMask layerMask;
        [SerializeField, Loc] private LocalizedString str;
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite altIcon;
        [SerializeReference, TypeInstance(true)] private IBuildTool left;
        [SerializeReference, TypeInstance(true)] private IBuildTool right;
        [SerializeReference, TypeInstance(true)] private IBuildTool altTool;

        public BuildLayerMask Layers => layerMask;
        public string DisplayName => str.GetLocalizedString();
        public Sprite Icon => icon;
        public Sprite AltIcon => altIcon;
        public IBuildTool Left => left;
        public IBuildTool Right => right;
        public IBuildTool AltTool => altTool;
    }
}