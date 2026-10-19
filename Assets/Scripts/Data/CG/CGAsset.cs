using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spotlight.Data.CG
{
    /// <summary>
    /// 单个 CG 分镜段：一张图（美术插槽）+ 字幕 / 旁白。
    /// </summary>
    [Serializable]
    public sealed class CGSegment
    {
        /// <summary>分镜图（美术插槽；留空显示黑色背景）。</summary>
        public Sprite image;

        /// <summary>字幕 / 旁白。</summary>
        [TextArea]
        public string subtitle;
    }

    /// <summary>
    /// CG 分镜数据档：一段由若干分镜组成的静态漫画式 CG。
    /// </summary>
    [CreateAssetMenu(fileName = "CGAsset", menuName = "Spotlight/Data/CG Sequence", order = 5)]
    public sealed class CGAsset : ScriptableObject
    {
        [SerializeField] private List<CGSegment> segments = new List<CGSegment>();

        /// <summary>分镜段列表（按播放顺序）。</summary>
        public IReadOnlyList<CGSegment> Segments => segments;

        /// <summary>追加一个分镜段（编辑器或代码创建数据档时使用）。</summary>
        public void AddSegment(Sprite image, string subtitle)
        {
            segments.Add(new CGSegment { image = image, subtitle = subtitle });
        }
    }
}
