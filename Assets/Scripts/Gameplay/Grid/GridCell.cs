using UnityEngine;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 网格中的单个格子数据：类型、坐标、世界位置与视觉方块引用，
    /// 以及可交互物体等附加数据（预留 Payload）。
    /// </summary>
    public sealed class GridCell
    {
        /// <summary>格子类型。</summary>
        public GridCellType Type { get; set; }

        /// <summary>列坐标。</summary>
        public int X { get; }

        /// <summary>行坐标。</summary>
        public int Y { get; }

        /// <summary>格子中心的世界位置。</summary>
        public Vector3 WorldPosition { get; set; }

        /// <summary>该格子的视觉方块实例（空 / 擦除时为 null）。</summary>
        public GameObject View { get; set; }

        /// <summary>附加数据（如可交互物体的具体逻辑引用），预留。</summary>
        public object Payload { get; set; }

        public GridCell(int x, int y)
        {
            X = x;
            Y = y;
            Type = GridCellType.Empty;
        }
    }
}
