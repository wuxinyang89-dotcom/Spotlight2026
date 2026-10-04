namespace Spotlight.Gameplay
{
    /// <summary>
    /// 玩法状态切换策略（预留接口）。
    /// 具体“如何切换”目前未定（可能为按键触发、场景加载、淡入淡出、相机过渡等），
    /// 统一抽象为本接口，后续实现后注入到 <see cref="GameplayModeManager"/> 即可。
    /// 若未注入，管理器将执行直接切换（无过渡、不阻断）。
    /// </summary>
    public interface IGameplayTransition
    {
        /// <summary>
        /// 切换前校验：返回 false 可阻断本次切换。
        /// 注意：from 在首次进入（尚无当前状态）时为 null。
        /// </summary>
        bool CanTransition(IGameplayState from, IGameplayState to);

        /// <summary>
        /// 执行切换过程（过渡效果、资源 / 场景加载等）。
        /// 具体切换方式为预留项，可在此实现淡入淡出、加载界面等。
        /// 注意：from 在首次进入（尚无当前状态）时为 null。
        /// </summary>
        void OnTransition(IGameplayState from, IGameplayState to);
    }
}
