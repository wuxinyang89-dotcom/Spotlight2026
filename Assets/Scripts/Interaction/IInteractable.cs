using Spotlight.Character;

namespace Spotlight.Interaction
{
    /// <summary>
    /// 可被人物交互的对象契约。NPC、可拾取物、门等均可实现该接口。
    /// 与人物（<see cref="ICharacter"/>）解耦，只依赖抽象。
    /// </summary>
    public interface IInteractable
    {
        /// <summary>交互提示名，例如“交谈”“开门”。</summary>
        string InteractionName { get; }

        /// <summary>指定人物当前是否可与此对象交互。</summary>
        bool CanInteract(ICharacter interactor);

        /// <summary>执行交互。</summary>
        void Interact(ICharacter interactor);
    }
}
