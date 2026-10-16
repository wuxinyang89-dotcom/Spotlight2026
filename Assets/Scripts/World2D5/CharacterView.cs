using UnityEngine;
using UnityEngine.InputSystem;
using Spotlight.Character;
using Spotlight.Interaction;
using Spotlight.UI;

namespace Spotlight.World2D5
{
    /// <summary>
    /// 2.5D 人物视图（玩家）：绑定 PlayerCharacter，读输入移动 + 按 F 交互，
    /// 同步朝向与动画（ICharacterAnimator 预留）。
    /// 带人物碰撞体（与图片尺寸相适应）与略大的交互触发体，均可在编辑器微调。
    /// </summary>
    public sealed class CharacterView : MonoBehaviour
    {
        [Header("外观")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("移动")]
        [SerializeField, Min(0.01f)] private float moveSpeed = 3f;

        [Header("人物碰撞体（与图片尺寸相适应）")]
        [SerializeField] private Vector2 bodySize = new Vector2(0.6f, 0.8f);

        [Header("交互触发体（略大，检测可交互目标）")]
        [SerializeField, Min(0.1f)] private float interactRadius = 1.4f;

        [Header("交互键")]
        [SerializeField] private Key interactKey = Key.F;

        private PlayerCharacter _character;
        private ICharacterAnimator _animator;
        private IInteractable _currentInteractable;

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

        private void Awake()
        {
            SetupColliders();
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

            if (Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
            {
                _character.TryInteract();
            }
        }

        private void SetupColliders()
        {
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body == null)
            {
                body = gameObject.AddComponent<Rigidbody2D>();
            }
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            BoxCollider2D bodyCollider = GetComponent<BoxCollider2D>();
            if (bodyCollider == null)
            {
                bodyCollider = gameObject.AddComponent<BoxCollider2D>();
            }
            bodyCollider.size = bodySize;

            CircleCollider2D trigger = GetComponent<CircleCollider2D>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<CircleCollider2D>();
            }
            trigger.isTrigger = true;
            trigger.radius = interactRadius;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TrySetInteractable(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TrySetInteractable(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            NPCView view = other.GetComponent<NPCView>();
            if (view != null && view.NPC != null && view.NPC == _currentInteractable)
            {
                ClearInteractable();
            }
        }

        private void TrySetInteractable(Collider2D other)
        {
            NPCView view = other.GetComponent<NPCView>();
            if (view == null || view.NPC == null)
            {
                return;
            }

            IInteractable interactable = view.NPC;
            if (_currentInteractable == interactable)
            {
                return;
            }

            _currentInteractable = interactable;
            _character?.SetInteractionTarget(interactable);
            InteractPromptPanel.ShowPrompt("按 F 对话");
        }

        private void ClearInteractable()
        {
            _currentInteractable = null;
            _character?.ClearInteractionTarget();
            InteractPromptPanel.HidePrompt();
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

