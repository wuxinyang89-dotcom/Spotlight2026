namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 网格操作接口（预留）。定义对网格的读写与高层“移动 / 交互”操作，
    /// 由 <see cref="Grid2D"/> 实现。玩家控制、寻路、交互系统等后续均通过本接口操作网格，
    /// 从而与具体实现解耦。
    /// </summary>
    public interface IGridOperation
    {
        int Width { get; }
        int Height { get; }
        float CellSize { get; }

        /// <summary>获取指定格子（越界返回 null）。</summary>
        GridCell GetCell(int x, int y);

        /// <summary>坐标是否在网格内。</summary>
        bool IsInBounds(int x, int y);

        /// <summary>是否可通行（非墙）。</summary>
        bool IsWalkable(int x, int y);

        /// <summary>设置格子类型并同步刷新对应视觉方块。</summary>
        void SetCellType(int x, int y, GridCellType type);

        /// <summary>清空整个网格。</summary>
        void ClearGrid();

        /// <summary>调整网格尺寸并重建。</summary>
        void SetSize(int width, int height);

        // ---------- 预留：高层操作 ----------

        /// <summary>预留：尝试把人物从 (fromX, fromY) 移动到 (toX, toY)。</summary>
        bool TryMovePlayer(int fromX, int fromY, int toX, int toY);

        /// <summary>预留：尝试与 (x, y) 处的可交互物体交互。</summary>
        bool TryInteract(int x, int y);
    }
}
