using System.Collections.Generic;

namespace Spotlight.Gameplay
{
    /// <summary>
    /// 玩法状态切换框架核心：注册各玩法状态、维护当前状态、执行切换。
    /// 纯逻辑类，不依赖 MonoBehaviour；由外部（如后续的 MonoBehaviour 桥接层）
    /// 以 deltaTime 驱动 <see cref="Tick"/>。
    /// 切换的“触发方”即调用 <see cref="SwitchTo"/> 的任意代码；
    /// 切换的“具体方式”由预留接口 <see cref="IGameplayTransition"/> 抽象。
    /// </summary>
    public sealed class GameplayModeManager
    {
        private readonly Dictionary<GameplayMode, IGameplayState> _states =
            new Dictionary<GameplayMode, IGameplayState>();

        private IGameplayTransition _transition;

        /// <summary>当前激活的玩法状态（未进入任何状态时为 null）。</summary>
        public IGameplayState Current { get; private set; }

        /// <summary>是否正在切换（执行切换过程期间为 true，便于后续扩展异步切换）。</summary>
        public bool IsSwitching { get; private set; }

        /// <summary>注册一个玩法状态（同模式重复注册会覆盖旧状态）。</summary>
        public void Register(IGameplayState state)
        {
            if (state == null)
            {
                return;
            }

            _states[state.Mode] = state;
        }

        /// <summary>移除指定玩法状态。</summary>
        public void Unregister(GameplayMode mode) => _states.Remove(mode);

        /// <summary>注入切换策略（预留接口）。传 null 则使用直接切换。</summary>
        public void SetTransition(IGameplayTransition transition) => _transition = transition;

        /// <summary>
        /// 切换到指定玩法状态。
        /// 未注册、正在切换中、或被切换策略阻断时返回 false。
        /// </summary>
        public bool SwitchTo(GameplayMode mode)
        {
            if (IsSwitching)
            {
                return false;
            }

            if (!_states.TryGetValue(mode, out IGameplayState target))
            {
                return false;
            }

            IGameplayState current = Current;

            // 已在目标状态，无需重复切换。
            if (current != null && current.Mode == mode)
            {
                return true;
            }

            // 预留：切换策略可在切换前阻断本次切换。
            if (_transition != null && !_transition.CanTransition(current, target))
            {
                return false;
            }

            IsSwitching = true;

            // 预留：执行具体切换过程（过渡、加载等）。
            _transition?.OnTransition(current, target);

            current?.OnExit();
            Current = target;
            target.OnEnter();

            IsSwitching = false;
            return true;
        }

        /// <summary>每帧驱动当前玩法状态。</summary>
        public void Tick(float deltaTime)
        {
            Current?.Tick(deltaTime);
        }
    }
}
