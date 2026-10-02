using UnityEngine;

namespace Spotlight.Movement
{
    /// <summary>
    /// 物理移动的抽象入口（预留）。具体实现（CharacterController、Rigidbody 或
    /// NavMeshAgent 等驱动）将在后续的 GameObject 层完成。
    /// 人物 / NPC 的逻辑层只依赖本接口，不直接接触 Transform 等 Unity 对象。
    /// </summary>
    public interface IMovementController
    {
        /// <summary>是否正在移动。</summary>
        bool IsMoving { get; }

        /// <summary>以指定方向与速度移动（direction 应为单位向量或零向量）。</summary>
        void Move(Vector3 direction, float speed);

        /// <summary>停止移动。</summary>
        void Stop();
    }
}
