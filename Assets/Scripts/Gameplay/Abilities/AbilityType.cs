namespace Spotlight.Gameplay.Abilities
{
    /// <summary>
    /// 能力类型。主角初始拥有的四种能力。
    /// 行为（每种能力做什么）由 <see cref="AbilitySystem"/> 解释；名称/简介/图标/数值参数由数据档提供。
    /// </summary>
    public enum AbilityType
    {
        /// <summary>Levity：减轻物体质量，使其更易推动 / 略微离地悬浮。</summary>
        Levity = 0,

        /// <summary>Melancholy：增加物体重量，使其能压下沉降机关 / 变得更脆弱易碎。</summary>
        Melancholy = 1,

        /// <summary>Fervor：物体移动时获得更大初始加速度 / 可撞碎脆弱物体。</summary>
        Fervor = 2,

        /// <summary>Isolation：对附近物体产生推力。</summary>
        Isolation = 3
    }
}
