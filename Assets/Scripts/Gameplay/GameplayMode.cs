namespace Spotlight.Gameplay
{
    /// <summary>
    /// 玩法模式标识。当前包含 3D 与 2D 两种子玩法；
    /// 后续新增子玩法时在此追加枚举值，并实现对应的 <see cref="IGameplayState"/>。
    /// </summary>
    public enum GameplayMode
    {
        /// <summary>3D 玩法。</summary>
        Mode3D = 0,

        /// <summary>2D 子玩法。</summary>
        Mode2D = 1
    }
}
