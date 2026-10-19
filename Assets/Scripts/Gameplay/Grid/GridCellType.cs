namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 网格方块类型。
    /// 人物格（Player）现作为“玩家出生点”标记：实际玩家是独立、平滑移动的物理实体，不占用格子。
    /// 墙（Wall）为不可通行地形；可交互物体（Interactable）为受能力影响的物理对象（障碍墙 / 机关）；
    /// 深坑（Pit）为危险地形（掉入重置），终点（Endpoint）为通关目标。
    /// 注意：在已有枚举值之后追加新值，保证既有布局资产（GridLayoutAsset）的序列化兼容。
    /// </summary>
    public enum GridCellType
    {
        /// <summary>空（无方块，默认）。</summary>
        Empty = 0,

        /// <summary>玩家出生点（全局唯一，玩家从此处生成，不再是移动中的格子）。</summary>
        Player = 1,

        /// <summary>墙（不可通行，带碰撞）。</summary>
        Wall = 2,

        /// <summary>可交互物体（受能力影响的物理对象）。</summary>
        Interactable = 3,

        /// <summary>深坑（危险地形，掉入重置到出生点）。</summary>
        Pit = 4,

        /// <summary>终点（通关目标）。</summary>
        Endpoint = 5
    }
}
