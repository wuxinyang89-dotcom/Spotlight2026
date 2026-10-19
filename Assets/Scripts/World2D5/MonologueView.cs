using UnityEngine;
using Spotlight.Character;
using Spotlight.Interaction;
using Spotlight.UI;

namespace Spotlight.World2D5
{
    /// <summary>
    /// 环境物品视图：可交互的固定物品，交互后显示一段独白 / 讲解（如「查看」）。
    /// 独白内容为数据（Inspector 配置），不写死。
    /// </summary>
    public sealed class MonologueView : MonoBehaviour, IInteractable
    {
        [Header("外观（可选，留空则无渲染）")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("独白")]
        [SerializeField] private string displayName = "物品";

        [TextArea]
        [SerializeField] private string monologueText = "……";

        [Header("交互触发体")]
        [SerializeField, Min(0.1f)] private float interactRadius = 1.2f;

        /// <inheritdoc />
        public string InteractionName => $"查看 {displayName}";

        /// <inheritdoc />
        public bool CanInteract(ICharacter interactor) => true;

        /// <inheritdoc />
        public void Interact(ICharacter interactor)
        {
            PopupDialog.Show(displayName, monologueText);
        }

        private void Awake()
        {
            CircleCollider2D trigger = GetComponent<CircleCollider2D>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<CircleCollider2D>();
            }
            trigger.isTrigger = true;
            trigger.radius = interactRadius;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }
    }
}
