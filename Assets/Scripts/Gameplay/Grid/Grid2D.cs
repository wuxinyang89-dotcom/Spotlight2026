using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 2D 玩法网格：管理网格尺寸、格子数据与视觉方块，实现 <see cref="IGridOperation"/>。
    /// 纯运行时组件；配置可在运行模式下通过 <see cref="GridDeveloperMode"/> 完成。
    /// 世界坐标约定：原点为网格左下角（本物体位置），X 向右、Y 向上，方块位于 XY 平面（Z=0）。
    /// </summary>
    public sealed class Grid2D : MonoBehaviour, IGridOperation
    {
        [Header("尺寸")]
        [SerializeField, Min(1)] private int width = 8;
        [SerializeField, Min(1)] private int height = 8;
        [SerializeField, Min(0.1f)] private float cellSize = 1f;

        [Header("方块预制体（可选，留空则使用默认彩色方块）")]
        [SerializeField] private GameObject playerBlockPrefab;
        [SerializeField] private GameObject wallBlockPrefab;
        [SerializeField] private GameObject interactableBlockPrefab;

        [Header("初始布局资产（可选，运行时会自动应用）")]
        [SerializeField] private GridLayoutAsset layoutAsset;

        private GridCell[,] _cells;
        private readonly List<Vector2Int> _playerCells = new List<Vector2Int>();
        private GameObject _ground;

        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;

        /// <summary>当前人物占用的格子（全局唯一，最多一个）。</summary>
        public IReadOnlyList<Vector2Int> PlayerCells => _playerCells;

        private void Awake()
        {
            Rebuild();

            if (layoutAsset != null)
            {
                ReadFrom(layoutAsset);
            }
        }

        /// <summary>把格子坐标转换为世界坐标。</summary>
        public Vector3 CellToWorld(int x, int y)
        {
            // 2D 世界空间：X 向右、Y 向上，所有瓦片位于 Z=0 平面。
            return transform.position + new Vector3(x * cellSize, y * cellSize, 0f);
        }

        /// <summary>重建网格：清空所有方块并生成底板。</summary>
        [ContextMenu("Rebuild")]
        public void Rebuild()
        {
            ClearVisuals();

            _cells = new GridCell[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _cells[x, y] = new GridCell(x, y)
                    {
                        WorldPosition = CellToWorld(x, y)
                    };
                }
            }

            CreateGround();
        }

        // ---------- IGridOperation ----------

        public GridCell GetCell(int x, int y) => IsInBounds(x, y) ? _cells[x, y] : null;

        public bool IsInBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

        public bool IsWalkable(int x, int y) => IsInBounds(x, y) && _cells[x, y].Type != GridCellType.Wall;

        public void SetCellType(int x, int y, GridCellType type)
        {
            if (!IsInBounds(x, y))
            {
                return;
            }

            GridCell cell = _cells[x, y];
            if (cell.Type == type)
            {
                return;
            }

            // 人物格全局唯一：放置新人物前先清掉旧人物。
            if (type == GridCellType.Player)
            {
                for (int i = _playerCells.Count - 1; i >= 0; i--)
                {
                    Vector2Int p = _playerCells[i];
                    SetCellTypeInternal(p.x, p.y, GridCellType.Empty);
                }

                _playerCells.Clear();
            }
            else if (cell.Type == GridCellType.Player)
            {
                _playerCells.Remove(new Vector2Int(x, y));
            }

            SetCellTypeInternal(x, y, type);

            if (type == GridCellType.Player)
            {
                _playerCells.Add(new Vector2Int(x, y));
            }
        }

        public void ClearGrid()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SetCellTypeInternal(x, y, GridCellType.Empty);
                }
            }

            _playerCells.Clear();
        }

        public void SetSize(int w, int h)
        {
            width = Mathf.Max(1, w);
            height = Mathf.Max(1, h);
            Rebuild();
        }

        /// <inheritdoc />
        public bool TryMovePlayer(int fromX, int fromY, int toX, int toY)
        {
            // 预留：具体移动规则（是否允许跨墙、是否受可交互物体阻挡等）后续实现。
            if (GetCell(fromX, fromY)?.Type != GridCellType.Player)
            {
                return false;
            }

            if (!IsWalkable(toX, toY))
            {
                return false;
            }

            SetCellType(toX, toY, GridCellType.Player);
            SetCellType(fromX, fromY, GridCellType.Empty);
            return true;
        }

        /// <inheritdoc />
        public bool TryInteract(int x, int y)
        {
            // 预留：与可交互物体的具体交互逻辑后续实现。
            GridCell cell = GetCell(x, y);
            return cell != null && cell.Type == GridCellType.Interactable;
        }

        // ---------- 视觉 ----------

        private void SetCellTypeInternal(int x, int y, GridCellType type)
        {
            GridCell cell = _cells[x, y];
            cell.Type = type;

            if (cell.View != null)
            {
                Destroy(cell.View);
                cell.View = null;
            }

            if (type != GridCellType.Empty)
            {
                cell.View = CreateBlock(cell, type);
            }
        }

        private static Sprite _sharedSquareSprite;

        /// <summary>1×1 白色方块精灵，用于程序化生成瓦片。</summary>
        private static Sprite SharedSquareSprite
        {
            get
            {
                if (_sharedSquareSprite == null)
                {
                    _sharedSquareSprite = Sprite.Create(
                        Texture2D.whiteTexture,
                        new Rect(0f, 0f, 1f, 1f),
                        new Vector2(0.5f, 0.5f),
                        1f);
                }

                return _sharedSquareSprite;
            }
        }

        private GameObject CreateBlock(GridCell cell, GridCellType type)
        {
            GameObject prefab = GetPrefab(type);
            GameObject block;

            if (prefab != null)
            {
                block = Instantiate(prefab, cell.WorldPosition, Quaternion.identity, transform);
            }
            else
            {
                block = CreateSpriteBlock(cell.WorldPosition, GetColor(type), 1, cellSize * 0.9f);
            }

            block.name = $"{type}_{cell.X}_{cell.Y}";
            block.transform.position = cell.WorldPosition;
            return block;
        }

        private GameObject GetPrefab(GridCellType type)
        {
            switch (type)
            {
                case GridCellType.Player: return playerBlockPrefab;
                case GridCellType.Wall: return wallBlockPrefab;
                case GridCellType.Interactable: return interactableBlockPrefab;
                default: return null;
            }
        }

        private GameObject CreateSpriteBlock(Vector3 worldPosition, Color color, int sortingOrder, float size)
        {
            GameObject go = new GameObject("Tile");
            go.transform.SetParent(transform, false);
            go.transform.position = worldPosition;
            go.transform.localScale = new Vector3(size, size, 1f);

            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = SharedSquareSprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return go;
        }

        private static Color GetColor(GridCellType type)
        {
            switch (type)
            {
                case GridCellType.Player: return new Color(0.2f, 0.85f, 0.4f);
                case GridCellType.Wall: return new Color(0.25f, 0.26f, 0.3f);
                case GridCellType.Interactable: return new Color(1f, 0.75f, 0.2f);
                default: return Color.white;
            }
        }

        private void CreateGround()
        {
            if (_ground != null)
            {
                Destroy(_ground);
            }

            GameObject ground = CreateSpriteBlock(
                transform.position + new Vector3(
                    (width - 1) * cellSize * 0.5f,
                    (height - 1) * cellSize * 0.5f,
                    0f),
                new Color(0.72f, 0.72f, 0.76f),
                0,
                1f);

            ground.name = "Background";
            ground.transform.localScale = new Vector3(width * cellSize, height * cellSize, 1f);
            _ground = ground;
        }

        private void ClearVisuals()
        {
            _playerCells.Clear();

            if (_cells != null)
            {
                foreach (GridCell cell in _cells)
                {
                    if (cell != null && cell.View != null)
                    {
                        Destroy(cell.View);
                    }
                }
            }

            if (_ground != null)
            {
                Destroy(_ground);
            }
        }

        // ---------- 序列化（供开发者模式保存 / 加载） ----------

        /// <summary>把当前网格布局序列化为 JSON。</summary>
        public string ToJson()
        {
            GridSaveData data = new GridSaveData { width = width, height = height };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (_cells[x, y].Type != GridCellType.Empty)
                    {
                        data.cells.Add(new GridCellData
                        {
                            x = x,
                            y = y,
                            type = (int)_cells[x, y].Type
                        });
                    }
                }
            }

            return JsonUtility.ToJson(data, true);
        }

        /// <summary>从 JSON 恢复网格布局（会先按尺寸重建）。</summary>
        public void LoadFromJson(string json)
        {
            GridSaveData data = JsonUtility.FromJson<GridSaveData>(json);
            if (data == null)
            {
                return;
            }

            if (data.width > 0)
            {
                width = data.width;
            }

            if (data.height > 0)
            {
                height = data.height;
            }

            Rebuild();

            foreach (GridCellData c in data.cells)
            {
                SetCellType(c.x, c.y, (GridCellType)c.type);
            }
        }

        // ---------- 资产导入 / 导出（ScriptableObject） ----------

        /// <summary>把当前网格布局写入资产。</summary>
        public void WriteTo(GridLayoutAsset asset)
        {
            if (asset == null)
            {
                return;
            }

            asset.width = width;
            asset.height = height;
            asset.cellSize = cellSize;
            asset.Clear();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (_cells[x, y].Type != GridCellType.Empty)
                    {
                        asset.SetCell(x, y, _cells[x, y].Type);
                    }
                }
            }
        }

        /// <summary>从资产恢复网格布局（会先按尺寸重建）。</summary>
        public void ReadFrom(GridLayoutAsset asset)
        {
            if (asset == null)
            {
                return;
            }

            width = Mathf.Max(1, asset.width);
            height = Mathf.Max(1, asset.height);
            cellSize = Mathf.Max(0.1f, asset.cellSize);
            Rebuild();

            foreach (GridCellEntry entry in asset.Cells)
            {
                SetCellType(entry.x, entry.y, entry.type);
            }
        }
    }

    [Serializable]
    public sealed class GridSaveData
    {
        public int width;
        public int height;
        public List<GridCellData> cells = new List<GridCellData>();
    }

    [Serializable]
    public sealed class GridCellData
    {
        public int x;
        public int y;
        public int type;
    }
}
