using Spotlight.Character;

namespace Spotlight.Interaction
{
    /// <summary>
    /// 默认交互器：仅保存 / 清除当前交互目标，并在 TryInteract 时校验并触发交互。
    /// 后续的 GameObject 层（射线、触发器）可替换或包装本实现以自动发现目标。
    /// </summary>
    public sealed class BasicInteractor : IInteractor
    {
        public IInteractable CurrentTarget { get; private set; }

        public void SetTarget(IInteractable target) => CurrentTarget = target;

        public void ClearTarget() => CurrentTarget = null;

        public bool TryInteract(ICharacter interactor)
        {
            if (CurrentTarget == null || !CurrentTarget.CanInteract(interactor))
            {
                return false;
            }

            CurrentTarget.Interact(interactor);
            return true;
        }
    }
}
