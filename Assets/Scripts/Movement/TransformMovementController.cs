using UnityEngine;

namespace Spotlight.Movement
{
    /// <summary>
    /// 基于 Transform 的移动控制器：把逻辑移动直接映射到 Transform 位移。
    /// 适用于 2.5D 世界（人物在 XY 平面移动）。由视图每帧调用一次。
    /// </summary>
    public sealed class TransformMovementController : IMovementController
    {
        private readonly Transform _transform;

        public bool IsMoving { get; private set; }

        public TransformMovementController(Transform transform)
        {
            _transform = transform;
        }

        public void Move(Vector3 direction, float speed)
        {
            if (_transform == null || speed <= 0f || direction.sqrMagnitude <= 0f)
            {
                Stop();
                return;
            }

            IsMoving = true;
            _transform.position += direction.normalized * (speed * Time.deltaTime);
        }

        public void Stop()
        {
            IsMoving = false;
        }
    }
}
