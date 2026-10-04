namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 网格方块类型。三类玩法方块：人物、墙、可交互物体，外加空（默认背景）。
    /// </summary>
    public enum GridCellType
    {
        /// <summary>空（无方块，默认）。</summary>
        Empty = 0,

        /// <summary>人物（玩家所在格，全局唯一）。</summary>
        Player = 1,

        /// <summary>墙（不可通行）。</summary>
        Wall = 2,

        /// <summary>可交互物体。</summary>
        Interactable = 3
    }
}
