using UnityEngine;

namespace Spotlight.Character
{
    /// <summary>
    /// 人物动画接入接口（预留）：人物视图通过本接口播放动画。
    /// 具体实现（Animator、Sprite 帧动画等）后续接入。
    /// </summary>
    public interface ICharacterAnimator
    {
        /// <summary>设置朝向（如左右翻转）。</summary>
        void SetFacing(Vector2 direction);

        /// <summary>播放待机动画。</summary>
        void PlayIdle();

        /// <summary>播放行走动画。</summary>
        void PlayWalk();
    }
}
