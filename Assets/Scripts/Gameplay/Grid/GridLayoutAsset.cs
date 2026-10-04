using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 网格布局资产：以 ScriptableObject 形式保存网格尺寸与格子类型。
    /// 可在 Project 窗口直接编辑，也可由运行中的开发者模式“导出”生成，
    /// 再通过 <see cref="Grid2D.ReadFrom"/> 应用回网格。
    /// </summary>
    [CreateAssetMenu(fileName = "GridLayout", menuName = "Spotlight/Grid Layout", order = 0)]
    public sealed class GridLayoutAsset : ScriptableObject
    {
        [Header("尺寸")]
        [Min(1)] public int width = 8;
        [Min(1)] public int height = 8;
        [Min(0.1f)] public float cellSize = 1f;

        [SerializeField] private List<GridCellEntry> cells = new List<GridCellEntry>();

        /// <summary>所有非空格子记录。</summary>
        public IReadOnlyList<GridCellEntry> Cells => cells;

        /// <summary>清空布局。</summary>
        public void Clear() => cells.Clear();

        /// <summary>设置某格类型（Empty 会移除该格记录）。</summary>
        public void SetCell(int x, int y, GridCellType type)
        {
            RemoveCell(x, y);

            if (type != GridCellType.Empty)
            {
                cells.Add(new GridCellEntry { x = x, y = y, type = type });
            }
        }

        /// <summary>读取某格类型（越界或未记录返回 Empty）。</summary>
        public GridCellType GetCell(int x, int y)
        {
            foreach (GridCellEntry entry in cells)
            {
                if (entry.x == x && entry.y == y)
                {
                    return entry.type;
                }
            }

            return GridCellType.Empty;
        }

        private void RemoveCell(int x, int y)
        {
            for (int i = cells.Count - 1; i >= 0; i--)
            {
                if (cells[i].x == x && cells[i].y == y)
                {
                    cells.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>网格布局中的单个格子记录。</summary>
    [Serializable]
    public sealed class GridCellEntry
    {
        public int x;
        public int y;
        public GridCellType type;
    }
}
