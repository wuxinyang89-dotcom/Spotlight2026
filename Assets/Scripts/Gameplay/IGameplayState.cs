namespace Spotlight.Gameplay
{
    /// <summary>
    /// 玩法状态契约。每个子玩法（3D / 2D / …）实现该接口，
    /// 由 <see cref="GameplayModeManager"/> 统一管理其进入、退出与逐帧更新。
    /// 本接口为纯逻辑契约，不涉及任何 GameObject。
    /// </summary>
    public interface IGameplayState
    {
        /// <summary>该状态对应的玩法模式。</summary>
        GameplayMode Mode { get; }

        /// <summary>进入该玩法状态时调用。</summary>
        void OnEnter();

        /// <summary>退出该玩法状态时调用。</summary>
        void OnExit();

        /// <summary>玩法状态每帧更新。</summary>
        void Tick(float deltaTime);
    }
}
