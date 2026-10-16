using UnityEngine;

namespace Spotlight.Movement
{
    /// <summary>
    /// 基于 Rigidbody2D 的移动控制器：通过设置速度驱动移动，由 2D 物理处理碰撞。
    /// 适用于 2.5D 世界（人物在 XY 平面移动并参与物理碰撞）。
    /// </summary>
    public sealed class Rigidbody2DMovementController : IMovementController
    {
        private readonly Rigidbody2D _body;

        public bool IsMoving { get; private set; }

        public Rigidbody2DMovementController(Rigidbody2D body)
        {
            _body = body;
        }

        public void Move(Vector3 direction, float speed)
        {
            if (_body == null || speed <= 0f || direction.sqrMagnitude <= 0f)
            {
                Stop();
                return;
            }

            IsMoving = true;
            Vector2 dir = new Vector2(direction.x, direction.y);
            if (dir.sqrMagnitude > 0f)
            {
                dir.Normalize();
            }
            _body.linearVelocity = dir * speed;
        }

        public void Stop()
        {
            IsMoving = false;
            if (_body != null)
            {
                _body.linearVelocity = Vector2.zero;
            }
        }
    }
}
