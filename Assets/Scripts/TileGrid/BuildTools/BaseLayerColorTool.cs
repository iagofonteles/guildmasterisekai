using System;
using System.Collections.Generic;
using Drafts.Inventory;
using UnityEngine;
using UnityEngine.Scripting;

namespace GuildMasterIsekai.TileGrid
{
    [Serializable, Preserve]
    public sealed class BaseLayerColorTool : IBuildTool
    {
        public IEnumerable<ISlot> GetItems(IBuildBag bag) => bag.GetItems<Color>();

        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            //todo
            //like in paint, color wall together with connected roof and vice verso
        }
    }

    [Serializable, Preserve]
    public sealed class BaseLayerColorPickTool : IBuildTool
    {
        public IEnumerable<ISlot> GetItems(IBuildBag bag) => bag.GetItems<Color>();
        public IBuildCommand CreateCommand(OperationArgs e, out string error)
        {
            //todo
            //see BaseLayerPickTool
        }
    }
}