using UnityEngine;

namespace Spotlight.Gameplay.States
{
    /// <summary>
    /// 3D 玩法状态（占位）。实际的 3D 玩法逻辑（人物移动、交互、NPC 等）
    /// 后续在此接入。
    /// </summary>
    public sealed class Gameplay3DState : IGameplayState
    {
        public GameplayMode Mode => GameplayMode.Mode3D;

        public void OnEnter()
        {
            Debug.Log("[Gameplay] 进入 3D 玩法");
            // TODO: 初始化 / 激活 3D 玩法相关逻辑。
        }

        public void OnExit()
        {
            Debug.Log("[Gameplay] 退出 3D 玩法");
            // TODO: 清理 / 停用 3D 玩法相关逻辑。
        }

        public void Tick(float deltaTime)
        {
            // TODO: 3D 玩法每帧更新挂载点。
        }
    }
}
