using UnityEngine;
using Spotlight.Gameplay.Abilities;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 可交互物体（障碍墙 / 机关等受能力影响的物理对象）的运行时组件。
    /// 挂在可交互方块上：持有刚体与“脆弱”状态，实现 <see cref="IAbilityTarget"/> 供能力系统操作。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class GridInteractable : MonoBehaviour, IAbilityTarget
    {
        [Header("基础质量（Melancholy 增加、Levity 减少）")]
        [SerializeField, Min(0.1f)] private float baseMass = 1f;

        private Rigidbody2D _body;
        private bool _fragile;

        /// <inheritdoc />
        public Rigidbody2D Body => _body;

        /// <inheritdoc />
        public bool Fragile => _fragile;

        /// <inheritdoc />
        public void MarkFragile() => _fragile = true;

        /// <inheritdoc />
        public void Break()
        {
            Destroy(gameObject);
        }

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            if (_body != null)
            {
                _body.mass = baseMass;
                _body.gravityScale = 0f;
                _body.freezeRotation = true;
            }
        }
    }
}
