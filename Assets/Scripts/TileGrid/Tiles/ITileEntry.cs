using System;
using Drafts.Database;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.Systems
{
    public interface ITileEntry : IDatabaseItemInternal
    {
        TileBase Tile { get; }
        int IDatabaseItem.Index => throw new InvalidOperationException();
        void IDatabaseItemInternal.SetIndex(int i) => throw new InvalidOperationException();
    }

    public interface ITileSo : IDbTile
    {
        ITileEntry Tile { get; }
    }
    
    public interface IPrefabTile : IDbTile
    {
        GameObject Prefab { get; }
    }

    public interface IDbTile : IDatabaseItem
    {
        
    }
    
    public interface IRoofedTile 
    {
        ITileEntry Roof { get; }
    }
}