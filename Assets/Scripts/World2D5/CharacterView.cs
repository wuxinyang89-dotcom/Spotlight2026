using UnityEngine;
using UnityEngine.InputSystem;
using Spotlight.Character;

namespace Spotlight.World2D5
{
    /// <summary>
    /// 2.5D 人物视图（玩家）：把 PlayerCharacter 逻辑绑定到 GameObject，
    /// 每帧读取输入驱动移动，并同步朝向与动画（动画经 ICharacterAnimator 预留）。
    /// </summary>
    public sealed class CharacterView : MonoBehaviour
    {
        [Header("外观")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("移动")]
        [SerializeField, Min(0.01f)] private float moveSpeed = 3f;

        private PlayerCharacter _character;
        private ICharacterAnimator _animator;

        /// <summary>世界位置。</summary>
        public Vector3 WorldPosition => transform.position;

        /// <summary>屏幕归一化位置（0..1，供对话映射）。</summary>
        public Vector2 ScreenPosition01
        {
            get
            {
                World2D5 world = World2D5.Instance;
                return world != null ? world.WorldToScreen01(transform.position) : new Vector2(0.5f, 0.5f);
            }
        }

        /// <summary>绑定逻辑人物与动画。</summary>
        public void Initialize(PlayerCharacter character, ICharacterAnimator animator)
        {
            _character = character;
            _animator = animator;
        }

        private void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void Update()
        {
            if (_character == null)
            {
                return;
            }

            Vector2 input = ReadMoveInput();

            if (input.sqrMagnitude > 0f)
            {
                Vector3 direction = new Vector3(input.x, input.y, 0f);
                _character.Move(direction, moveSpeed);
                ApplyFacing(input.x);
                _animator?.SetFacing(direction);
                _animator?.PlayWalk();
            }
            else
            {
                _character.StopMoving();
                _animator?.PlayIdle();
            }
        }

        private void ApplyFacing(float x)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            if (x < 0f)
            {
                spriteRenderer.flipX = true;
            }
            else if (x > 0f)
            {
                spriteRenderer.flipX = false;
            }
        }

        private static Vector2 ReadMoveInput()
        {
            if (Keyboard.current == null)
            {
                return Vector2.zero;
            }

            Keyboard kb = Keyboard.current;
            Vector2 input = Vector2.zero;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1f;
            return input.sqrMagnitude > 0f ? input.normalized : Vector2.zero;
        }
    }
}
