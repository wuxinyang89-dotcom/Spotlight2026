using UnityEngine;
using Spotlight.Gameplay;
using Spotlight.UI;
using Spotlight.World2D5;

namespace Spotlight.Gameplay.States
{
    /// <summary>
    /// 3D 玩法状态。进入时显示常驻 HUD 并初始化 2.5D 世界，退出时隐藏 HUD。
    /// 实际的 3D 玩法逻辑（人物移动、交互、NPC 等）后续在此接入。
    /// </summary>
    public sealed class Gameplay3DState : IGameplayState
    {
        public GameplayMode Mode => GameplayMode.Mode3D;

        public void OnEnter()
        {
            Debug.Log("[Gameplay] 进入 3D 玩法");
            HudPanel.ShowHud();
            World2D5Bootstrap.Initialize();
            StoryFlow.PlayIntro(); // 开场 CG（无数据档则直接跳过）
        }

        public void OnExit()
        {
            Debug.Log("[Gameplay] 退出 3D 玩法");
            HudPanel.HideHud();
            // TODO: 清理 / 停用 3D 玩法相关逻辑。
        }

        public void Tick(float deltaTime)
        {
            // TODO: 3D 玩法每帧更新挂载点。
        }
    }
}

