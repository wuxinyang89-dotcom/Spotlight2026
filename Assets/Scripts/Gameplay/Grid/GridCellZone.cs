using UnityEngine;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 网格触发器区域：挂在深坑（Pit）/ 终点（Endpoint）方块上，
    /// 供玩家进入时触发（掉落重置 / 通关）。由 <see cref="Grid2D"/> 生成时配置。
    /// </summary>
    public sealed class GridCellZone : MonoBehaviour
    {
        /// <summary>区域类型（Pit / Endpoint）。</summary>
        public GridCellType Type { get; private set; }

        /// <summary>格子坐标 X。</summary>
        public int X { get; private set; }

        /// <summary>格子坐标 Y。</summary>
        public int Y { get; private set; }

        /// <summary>配置区域类型与坐标。</summary>
        public void Setup(GridCellType type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }
    }
}
