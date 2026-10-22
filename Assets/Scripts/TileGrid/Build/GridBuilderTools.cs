using System;
using System.Collections.Generic;
using Drafts;
using Drafts.Localization;
using UnityEngine;
using UnityEngine.Localization;

namespace TileGrid.Build
{
    public class GridBuilderTools : ScriptableObject
    {
        [SerializeField] private LayerConfig[] layers;
        [SerializeReference, TypeInstance] private IBuildBrush[] bruhes;
        [SerializeReference, TypeInstance] private IBuildTool[] tools;

        public IReadOnlyList<LayerConfig> Layers => layers;
        public IReadOnlyList<IBuildBrush> Brushes => bruhes;
        public IReadOnlyList<IBuildTool> Tools => tools;
    }

    [Serializable]
    public class LayerConfig
    {
        [SerializeField] private int index;
        [SerializeField, Loc] private LocalizedString str;
        [SerializeField] private Sprite icon;

        public int Index => index;
        public string DisplayName => str.GetLocalizedString();
        public Sprite Icon => icon;
    }
}