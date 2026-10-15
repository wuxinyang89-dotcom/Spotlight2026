using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spotlight.Data.Dialogue
{
    /// <summary>
    /// 对话数据档：一段对话由若干条台词按顺序组成。
    /// 每条台词包含说话人与台词文本；由交互物（NPC、道具）引用后，通过对话面板逐条播放。
    /// </summary>
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Spotlight/Data/Dialogue", order = 0)]
    public sealed class DialogueAsset : ScriptableObject
    {
        [Header("台词（按播放顺序）")]
        [SerializeField] private List<DialogueLine> lines = new List<DialogueLine>();

        /// <summary>台词列表（按播放顺序）。</summary>
        public IReadOnlyList<DialogueLine> Lines => lines;

        /// <summary>追加一条台词（编辑器或代码创建对话时使用）。</summary>
        public void AddLine(string speaker, string text)
        {
            lines.Add(new DialogueLine { speaker = speaker, text = text });
        }
    }

    /// <summary>一条台词：说话人与台词文本。</summary>
    [Serializable]
    public sealed class DialogueLine
    {
        /// <summary>说话人名称。</summary>
        public string speaker;

        /// <summary>台词文本。</summary>
        public string text;
    }
}

