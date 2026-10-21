using System;
using System.Collections.Generic;
using TileGrid.Data;
using TileGrid.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileGrid.Build
{
    public sealed class GridRenderer : MonoBehaviour
    {
        [SerializeField] private GridBuilder builder;
        [SerializeField] private Tilemap[] layers;
        private GridData _grid;
        private Transform _toppingRoot;
        private HashSet<Vector3Int>[] _refresh = Array.Empty<HashSet<Vector3Int>>();
        private readonly Dictionary<Vector2Int, RenderedTopping[]> _toppings = new();

        private sealed class RenderedTopping
        {
            public IDecorationEntry tile;
            public GameObject instance;

            public RenderedTopping(IDecorationEntry tile, GameObject instance)
            {
                this.tile = tile;
                this.instance = instance;
            }
        }

        private void SetGrid(GridData grid)
        {
            if (grid != null)
                if (layers?.Length == BuildLayer.Count)
                    throw new Exception($"Need {BuildLayer.Count} layers");

            if (_toppingRoot)
            {
                _toppingRoot.gameObject.SetActive(false);
                _toppingRoot.SetParent(null, false);
                DisposeObject(_toppingRoot.gameObject);
            }

            _toppings.Clear();
            _refresh = Array.Empty<HashSet<Vector3Int>>();
            _grid = grid;
            if (grid == null) return;

            _refresh = new HashSet<Vector3Int>[layers.Length];
            _toppingRoot = new GameObject("Runtime Toppings").transform;
            _toppingRoot.SetParent(layers[0].transform, false);
            for (var i = 0; i < layers.Length; i++)
                _refresh[i] = new HashSet<Vector3Int>();

            // Replacement snapshots need a full render even when the dirty set is empty.
            foreach (var key in grid.Indexes.allPositionsWithin)
                grid.Modified.Add((GridCell)grid[key.x, key.y]);
        }

        private void Update()
        {
            if (!builder) return;
            if (_grid != builder.Grid) SetGrid(builder.Grid);
            if (_grid == null || _grid.Modified.Count == 0) return;

            foreach (var cell in _grid.Modified)
            {
                var at = CellPosition(cell.Key);

                for (var i = 0; i < layers.Length; i++)
                {
                    SetChangedTile(layers[i], at, cell.Tiles[i], _refresh[i]);
                    _refresh[i].Add(at);
                }

                RenderToppings(cell);
            }

            // Refresh neighbors after all writes, then restore colors and unlock LockColor.
            for (var i = 0; i < layers.Length; i++)
                foreach (var at in _refresh[i])
                    layers[i].RefreshTile(at);

            for (var i = 0; i < layers.Length; i++)
            {
                foreach (var at in _refresh[i])
                {
                    var key = new Vector2Int(at.x - _grid.Bounds.x, at.y - _grid.Bounds.y);
                    if (!_grid.Contains(key)) continue;
                    //_maps[i].RemoveTileFlags(at, TileFlags.LockColor);
                    layers[i].SetColor(at, _grid[key.x, key.y].Colors[i]);
                }

                _refresh[i].Clear();
            }

            _grid.Modified.Clear();
        }

        private Vector3Int CellPosition(Vector2Int key) =>
            new(key.x + _grid.Bounds.x, key.y + _grid.Bounds.y, _grid.Bounds.z);

        private void OnDestroy() => SetGrid(null);

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

            var count = cell.Decoration.Count;
            if (rendered == null && count == 0) return;
            if (rendered == null) rendered = Array.Empty<RenderedTopping>();
            for (var i = count; i < rendered.Length; i++) DestroyTopping(rendered[i]);
            if (rendered.Length != count) Array.Resize(ref rendered, count);
            // Keep the cache current even if creating a topping fails partway through a cell.
            if (count == 0) _toppings.Remove(cell.Key);
            else _toppings[cell.Key] = rendered;

            for (var i = 0; i < count; i++)
            {
                var topping = cell.Decoration[i];
                var desired = topping.item;
                var current = rendered[i];

                if (current != null && (current.tile != desired || !current.instance))
                {
                    DestroyTopping(current);
                    rendered[i] = current = null;
                }

                if (desired == null) continue;

                if (current == null)
                {
                    var instance = Instantiate(desired.Prefab, _toppingRoot);
                    rendered[i] = current = new RenderedTopping(desired, instance);
                }

                current.instance.transform.localPosition = topping.position;
            }
        }

        private static void DestroyTopping(RenderedTopping topping)
        {
            if (topping == null || !topping.instance) return;
            topping.instance.SetActive(false);
            DisposeObject(topping.instance);
        }

        private static void DisposeObject(UnityEngine.Object obj)
        {
            if (Application.isPlaying) Destroy(obj);
            else DestroyImmediate(obj);
        }
    }
}