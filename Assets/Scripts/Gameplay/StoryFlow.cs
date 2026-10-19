using System;
using UnityEngine;
using Spotlight.Data.CG;
using Spotlight.UI;

namespace Spotlight.Gameplay
{
    /// <summary>
    /// 剧情流程（静态）：管理序章开场 CG 等流程编排。
    /// 开场 CG 数据档优先（Resources/CG/Intro），无则直接跳过。
    /// </summary>
    public static class StoryFlow
    {
        private const string IntroCGPath = "CG/Intro";

        private static bool _introPlayed;

        /// <summary>
        /// 播放开场 CG（仅首次）。结束后回调 onComplete。
        /// </summary>
        public static void PlayIntro(Action onComplete = null)
        {
            if (_introPlayed)
            {
                onComplete?.Invoke();
                return;
            }

            _introPlayed = true;

            CGAsset asset = Resources.Load<CGAsset>(IntroCGPath);
            if (asset == null || asset.Segments.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            CGPlayer.Play(asset, onComplete);
        }
    }
}
