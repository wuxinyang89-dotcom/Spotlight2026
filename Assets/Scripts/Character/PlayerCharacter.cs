using UnityEngine;
using Spotlight.Interaction;
using Spotlight.Movement;

namespace Spotlight.Character
{
    /// <summary>
    /// 3D 人物的逻辑实现：负责行走状态与交互流程的调度。
    /// 实际“物理移动”委托给 <see cref="IMovementController"/>（预留实现），
    /// “交互目标管理”委托给 <see cref="IInteractor"/>。
    /// 本类为纯逻辑类，不持有任何 GameObject 引用。
    /// </summary>
    public sealed class PlayerCharacter : ICharacter
    {
        private readonly IMovementController _movementController;
        private readonly IInteractor _interactor;

        public string Name { get; }

        public Vector3 MoveDirection { get; private set; }

        public float MoveSpeed { get; private set; }

        public bool IsMoving => MoveSpeed > 0f && MoveDirection.sqrMagnitude > 0f;

        public PlayerCharacter(string name, IMovementController movementController, IInteractor interactor)
        {
            Name = name;
            _movementController = movementController;
            _interactor = interactor;
        }

        // ---------- 行走逻辑 ----------

        /// <inheritdoc />
        public void Move(Vector3 direction, float speed)
        {
            // 无有效输入则视为停止，避免产生零向量移动。
            if (speed <= 0f || direction.sqrMagnitude <= 0f)
            {
                StopMoving();
                return;
            }

            MoveDirection = direction.normalized;
            MoveSpeed = speed;
            _movementController.Move(MoveDirection, MoveSpeed);
        }

        /// <inheritdoc />
        public void StopMoving()
        {
            MoveDirection = Vector3.zero;
            MoveSpeed = 0f;
            _movementController.Stop();
        }

        // ---------- 交互逻辑 ----------

        /// <summary>当前交互目标（无则为 null）。</summary>
        public IInteractable CurrentInteractionTarget => _interactor.CurrentTarget;

        /// <summary>设置当前交互目标。</summary>
        public void SetInteractionTarget(IInteractable target) => _interactor.SetTarget(target);

        /// <summary>清除当前交互目标。</summary>
        public void ClearInteractionTarget() => _interactor.ClearTarget();

        /// <inheritdoc />
        public bool TryInteract() => _interactor.TryInteract(this);
    }
}
