using UnityEngine;

namespace Spotlight.Character
{
    /// <summary>
    /// 可操控 3D 人物的最小行为契约：行走与交互。
    /// 只描述逻辑行为，不涉及任何 GameObject 细节。
    /// </summary>
    public interface ICharacter
    {
        /// <summary>人物名称。</summary>
        string Name { get; }

        /// <summary>当前行走方向（单位向量；停止时为 <see cref="Vector3.zero"/>）。</summary>
        Vector3 MoveDirection { get; }

        /// <summary>当前行走速度。</summary>
        float MoveSpeed { get; }

        /// <summary>是否正在行走。</summary>
        bool IsMoving { get; }

        /// <summary>向 direction 方向以 speed 速度行走。</summary>
        void Move(Vector3 direction, float speed);

        /// <summary>停止行走。</summary>
        void StopMoving();

        /// <summary>与当前交互目标进行交互，成功返回 true。</summary>
        bool TryInteract();
    }
}
