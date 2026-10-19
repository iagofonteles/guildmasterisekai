using System;
using System.Collections.Generic;
using GuildMasterIsekai.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuildMasterIsekai.TileGrid
{
    public sealed class GridRenderer : MonoBehaviour
    {
        [SerializeField] private GridBuilder builder;
        [SerializeField] private Tilemap source;
        private GridData _grid;
        private Tilemap _baseMap;
        private Tilemap _furnitureMap;
        private Transform _toppingRoot;
        private readonly HashSet<Vector3Int> _baseRefresh = new();
        private readonly HashSet<Vector3Int> _furnitureRefresh = new();
        private readonly Dictionary<Vector2Int, RenderedTopping[]> _toppings = new();

        private sealed class RenderedTopping
        {
            public IPrefabTile Definition;
            public GameObject Instance;
        }

        private void SetGrid(GridData grid)
        {
            if (_baseMap) Destroy(_baseMap.gameObject);
            if (_furnitureMap) Destroy(_furnitureMap.gameObject);
            if (_toppingRoot) Destroy(_toppingRoot.gameObject);
            _toppings.Clear();
            _baseRefresh.Clear();
            _furnitureRefresh.Clear();

            _grid = grid;
            _baseMap = CreateMap(0, source);
            _furnitureMap = CreateMap(1, source);
            _toppingRoot = new GameObject("Runtime Toppings").transform;
            _toppingRoot.SetParent(_baseMap.transform, false);
        }

        private void Update()
        {
            if (builder.Grid == null) return;
            if (builder.Grid.Modified.Count == 0) return;
            if (_grid != builder.Grid) SetGrid(builder.Grid);

            foreach (var cell in _grid!.Modified)
            {
                var p = cell.Key;
                var at = new Vector3Int(p.x + _grid.Bounds.x, p.y + _grid.Bounds.y, _grid.Bounds.z);
                SetChangedTile(_baseMap, at, cell.BaseTile, _baseRefresh);
                SetChangedTile(_furnitureMap, at, cell.Furniture?.Tile, _furnitureRefresh);
                RenderToppings(cell);
            }

            // AutoTile uses the eight surrounding cells. Refresh after all writes are visible.
            foreach (var at in _baseRefresh) _baseMap.RefreshTile(at);
            foreach (var at in _furnitureRefresh) _furnitureMap.RefreshTile(at);
            // Refreshing tiles can restore LockColor; apply cell colors afterwards.
            _baseRefresh.UnionWith(_furnitureRefresh);
            foreach (var cell in _grid.Modified)
                _baseRefresh.Add(new Vector3Int(cell.Key.x + _grid.Bounds.x,
                    cell.Key.y + _grid.Bounds.y, _grid.Bounds.z));
            foreach (var at in _baseRefresh)
            {
                var key = new Vector2Int(at.x - _grid.Bounds.x, at.y - _grid.Bounds.y);
                if (!_grid.Contains(key)) continue;
                var cell = _grid[key.x, key.y];
                _baseMap.SetColor(at, cell.BaseColor);
                _furnitureMap.SetColor(at, cell.FurnitureColor);
            }
            _grid.Modified.Clear();
            _baseRefresh.Clear();
            _furnitureRefresh.Clear();
        }

        private Tilemap CreateMap(int order, Tilemap src)
        {
            var go = new GameObject(order == 0 ? "Base" : "Furniture");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = src.transform.localPosition;
            go.transform.localRotation = src.transform.localRotation;
            go.transform.localScale = src.transform.localScale;
            go.layer = src.gameObject.layer;

            var map = go.AddComponent<Tilemap>();
            map.color = src.color;
            map.tileAnchor = src.tileAnchor;
            map.orientation = src.orientation;
            map.orientationMatrix = src.orientationMatrix;

            var original = src.GetComponent<TilemapRenderer>();
            var renderer = go.AddComponent<TilemapRenderer>();
            renderer.sharedMaterials = original.sharedMaterials;
            renderer.sortingLayerID = original.sortingLayerID;
            renderer.sortingOrder = original.sortingOrder + order;
            renderer.sortOrder = original.sortOrder;

            return map;
        }

        private static void SetChangedTile(Tilemap map, Vector3Int at,
            ITileEntry tile, HashSet<Vector3Int> refresh)
        {
            if (map.GetTile(at) == tile?.Tile) return;
            map.SetTile(at, tile?.Tile);
            for (var y = -1; y <= 1; y++)
            for (var x = -1; x <= 1; x++)
                refresh.Add(at + new Vector3Int(x, y, 0));
        }

        private void RenderToppings(GridCell cell)
        {
            _toppings.TryGetValue(cell.Key, out var rendered);

            var count = cell.Furniture != null ? cell.docor.Count : 0;
            if (rendered == null && count == 0) return;
            if (rendered == null) rendered = Array.Empty<RenderedTopping>();
            for (var i = count; i < rendered.Length; i++) DestroyTopping(rendered[i]);
            if (rendered.Length != count) Array.Resize(ref rendered, count);
            // Keep the cache current even if creating a topping fails partway through a cell.
            if (count == 0) _toppings.Remove(cell.Key);
            else _toppings[cell.Key] = rendered;

            for (var i = 0; i < count; i++)
            {
                var topping = cell.docor[i];
                var desired = topping.item;
                
                var current = rendered[i];

                if (current != null && (current.Definition != desired || !current.Instance))
                {
                    DestroyTopping(current);
                    rendered[i] = current = null;
                }

                if (desired == null) continue;

                if (current == null)
                {
                    var instance = Instantiate(desired.Prefab, _toppingRoot);
                    rendered[i] = current = new RenderedTopping { Definition = desired, Instance = instance };
                }

                current.Instance.transform.localPosition = topping.position;
            }
        }

        private Vector3 SlotWorld(GridData state, Vector2Int p, Vector2 offset) =>
            _baseMap.GetCellCenterWorld(new Vector3Int(p.x + state.Bounds.x, p.y + state.Bounds.y, state.Bounds.z)) +
            _baseMap.transform.TransformVector(new Vector3(offset.x, offset.y, 0));

        private static void DestroyTopping(RenderedTopping topping)
        {
            if (topping == null || !topping.Instance) return;
            topping.Instance.SetActive(false);
            DisposeObject(topping.Instance);
        }

        private static void DisposeObject(UnityEngine.Object obj)
        {
            if (Application.isPlaying) Destroy(obj);
            else DestroyImmediate(obj);
        }
    }
}
