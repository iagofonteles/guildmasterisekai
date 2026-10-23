using System.Collections;
using NavMeshPlus.Components;
using TileGrid.Build;
using TileGrid.Data;
using UnityEngine;

namespace GuildMasterIsekai
{
    public class GuildNavigation : MonoBehaviour
    {
        [SerializeField] private GridBuilder builder;
        [SerializeField] private NavMeshSurface surface;
        
        private GridData Grid => builder.Grid;
        
        private IEnumerator Start()
        {
            while (!builder.IsInitialized) yield return null;

        }
    }
}