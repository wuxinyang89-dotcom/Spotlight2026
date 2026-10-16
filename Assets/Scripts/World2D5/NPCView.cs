using UnityEngine;
using Spotlight.NPC;
using NPCEntity = Spotlight.NPC.NPC;

namespace Spotlight.World2D5
{
    /// <summary>
    /// 2.5D NPC 视图：把 NPC 逻辑绑定到 GameObject，提供世界位置与屏幕位置（供对话映射）。
    /// 带人物碰撞体（与图片尺寸相适应，可在编辑器微调）。NPC 移动（INPCMovement）后续接入。
    /// </summary>
    public sealed class NPCView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("人物碰撞体（与图片尺寸相适应）")]
        [SerializeField] private Vector2 bodySize = new Vector2(0.6f, 0.8f);

        private NPCEntity _npc;

        /// <summary>绑定的 NPC。</summary>
        public NPCEntity NPC => _npc;

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

        public void Initialize(NPCEntity npc)
        {
            _npc = npc;
        }

        private void Awake()
        {
            SetupCollider();
        }

        private void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void SetupCollider()
        {
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body == null)
            {
                body = gameObject.AddComponent<Rigidbody2D>();
            }
            body.bodyType = RigidbodyType2D.Kinematic;

            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider2D>();
            }
            col.size = bodySize;
        }
    }
}


