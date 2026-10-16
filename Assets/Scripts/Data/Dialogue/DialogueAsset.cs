using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spotlight.Data.Dialogue
{
    /// <summary>
    /// 对话数据档：一段对话由若干条台词按顺序组成。
    /// 说话者可为任意角色（NPC 或玩家），对话可能不含玩家（例如旁观 / 偷听 NPC 间聊天）。
    /// 由交互物（NPC、道具）引用后，通过对话面板逐条播放；每句台词按说话者位置做映射。
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
        /// <summary>说话者标识（任意角色名，NPC 或玩家；旁观对话中可不含玩家）。</summary>
        public string speaker;

        /// <summary>台词文本。</summary>
        public string text;
    }
}

