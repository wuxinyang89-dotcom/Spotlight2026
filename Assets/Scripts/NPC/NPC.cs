using UnityEngine;
using Spotlight.Character;
using Spotlight.Interaction;
using Spotlight.NPC.Attributes;

namespace Spotlight.NPC
{
    /// <summary>
    /// NPC 逻辑实现。交互（<see cref="INPCInteraction"/>）与移动（<see cref="INPCMovement"/>）
    /// 均以接口预留；属性由 <see cref="NPCAttributeSet"/> 组合而成。
    /// 同时实现 <see cref="IInteractable"/>，使人物可与之交互。
    /// 本类为纯逻辑类，不持有任何 GameObject 引用。
    /// </summary>
    public sealed class NPC : IInteractable
    {
        public string Name { get; }

        /// <summary>NPC 的属性组合。</summary>
        public NPCAttributeSet Attributes { get; }

        /// <summary>交互行为（预留）。</summary>
        public INPCInteraction Interaction { get; }

        /// <summary>移动行为（预留）。</summary>
        public INPCMovement Movement { get; }

        public NPC(
            string name,
            NPCAttributeSet attributes,
            INPCInteraction interaction = null,
            INPCMovement movement = null)
        {
            Name = name;
            Attributes = attributes ?? new NPCAttributeSet();
            Interaction = interaction;
            Movement = movement;
        }

        // ---------- 属性便捷访问 ----------

        /// <summary>种族属性（未设置则为 null）。</summary>
        public RaceAttribute Race => Attributes.Get<RaceAttribute>(NPCAttributeType.Race);

        /// <summary>性格属性（未设置则为 null）。</summary>
        public PersonalityAttribute Personality => Attributes.Get<PersonalityAttribute>(NPCAttributeType.Personality);

        // ---------- 移动（预留接口透传） ----------

        public void MoveTo(Vector3 destination) => Movement?.MoveTo(destination);

        public void StopMoving() => Movement?.Stop();

        // ---------- IInteractable ----------

        public string InteractionName => $"与 {Name} 交谈";

        public bool CanInteract(ICharacter interactor) => true;

        public void Interact(ICharacter interactor) => Interaction?.OnInteract(interactor);
    }
}
