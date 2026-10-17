using System.Collections;
using System.Collections.Generic;

namespace GuildMasterIsekai.TileGrid
{
    /// <summary>Prepares a command without changing the live grid or inventory.</summary>
    public interface IBuildTool
    {
        bool RequiresItem { get; }
        IEnumerable<object> GetCompatibleItems(IBuildBag bag);

        /// <returns>Null on error</returns>
        IBuildCommand CreateCommand(OperationArgs e, out string error);
    }
}