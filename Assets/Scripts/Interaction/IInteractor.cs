using Spotlight.Character;

namespace Spotlight.Interaction
{
    /// <summary>
    /// 交互目标的发现与管理入口。
    /// 具体实现可以由射线检测 / 触发器获得目标，也可以是玩家手动指定；
    /// 当前提供默认实现 <see cref="BasicInteractor"/>。
    /// </summary>
    public interface IInteractor
    {
        /// <summary>当前交互目标（无则为 null）。</summary>
        IInteractable CurrentTarget { get; }

        /// <summary>设置交互目标。</summary>
        void SetTarget(IInteractable target);

        /// <summary>清除交互目标。</summary>
        void ClearTarget();

        /// <summary>尝试对当前目标执行交互。无目标或不可交互时返回 false。</summary>
        bool TryInteract(ICharacter interactor);
    }
}
