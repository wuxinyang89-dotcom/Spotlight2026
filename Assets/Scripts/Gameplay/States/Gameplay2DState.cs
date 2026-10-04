using UnityEngine;

namespace Spotlight.Gameplay.States
{
    /// <summary>
    /// 2D 子玩法状态（占位）。具体的 2D 玩法逻辑（待定）后续在此接入。
    /// </summary>
    public sealed class Gameplay2DState : IGameplayState
    {
        public GameplayMode Mode => GameplayMode.Mode2D;

        public void OnEnter()
        {
            Debug.Log("[Gameplay] 进入 2D 玩法");
            // TODO: 初始化 / 激活 2D 玩法相关逻辑。
        }

        public void OnExit()
        {
            Debug.Log("[Gameplay] 退出 2D 玩法");
            // TODO: 清理 / 停用 2D 玩法相关逻辑。
        }

        public void Tick(float deltaTime)
        {
            // TODO: 2D 玩法每帧更新挂载点。
        }
    }
}
