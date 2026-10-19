using UnityEngine;

namespace Spotlight.Gameplay.Abilities
{
    /// <summary>
    /// 可受能力影响的物理对象契约。由能力系统（<see cref="AbilitySystem"/>）操作，
    /// 使能力层与具体的“可交互物体”实现解耦（能力层不依赖网格层）。
    /// </summary>
    public interface IAbilityTarget
    {
        /// <summary>对象的刚体（能力直接施加物理效果）。</summary>
        Rigidbody2D Body { get; }

        /// <summary>当前是否处于“脆弱”状态（Melancholy 施加后为 true）。</summary>
        bool Fragile { get; }

        /// <summary>标记为“脆弱”（Melancholy 的行为）。</summary>
        void MarkFragile();

        /// <summary>打碎该对象（Fervor 对脆弱对象的行为）。</summary>
        void Break();
    }
}
