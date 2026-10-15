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
    /// 实现 <see cref="INPCInteraction"/>，作为 NPC 的交互行为注入。
    /// 可携带"说话者屏幕位置提供者"，用于把对话弹窗位置映射到说话者位置。
    /// </summary>
    public sealed class DialogueInteraction : INPCInteraction
    {
        private readonly DialogueAsset _dialogue;
        private readonly Func<Vector2> _speakerScreenPositionProvider;

        public DialogueInteraction(DialogueAsset dialogue, Func<Vector2> speakerScreenPositionProvider = null)
        {
            _dialogue = dialogue;
            _speakerScreenPositionProvider = speakerScreenPositionProvider;
        }

        public void OnInteract(ICharacter character)
        {
            if (_dialogue == null || _dialogue.Lines.Count == 0)
            {
                return;
            }

            if (_speakerScreenPositionProvider != null)
            {
                DialoguePanel.SetSpeakerPosition(_speakerScreenPositionProvider());
            }

            DialoguePanel.Play(_dialogue);
        }
    }
}

