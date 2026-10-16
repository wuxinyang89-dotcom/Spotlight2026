using System;
using UnityEngine;
using Spotlight.Character;
using Spotlight.Data.Dialogue;
using Spotlight.NPC;
using Spotlight.UI;

namespace Spotlight.Dialogue
{
    /// <summary>
    /// 对话交互：NPC 被交互时播放其关联的对话（通过对话面板逐条显示）。
    /// 实现 <see cref="INPCInteraction"/>。可携带"说话者名字 → 屏幕位置"解析器，
    /// 支持多说话者位置映射（含旁观 NPC 间对话）。
    /// </summary>
    public sealed class DialogueInteraction : INPCInteraction
    {
        private readonly DialogueAsset _dialogue;
        private readonly Func<string, Vector2> _speakerScreenPositionResolver;

        public DialogueInteraction(DialogueAsset dialogue, Func<string, Vector2> speakerScreenPositionResolver = null)
        {
            _dialogue = dialogue;
            _speakerScreenPositionResolver = speakerScreenPositionResolver;
        }

        public void OnInteract(ICharacter character)
        {
            if (_dialogue == null || _dialogue.Lines.Count == 0)
            {
                return;
            }

            DialoguePanel.Play(_dialogue, _speakerScreenPositionResolver);
        }
    }
}


