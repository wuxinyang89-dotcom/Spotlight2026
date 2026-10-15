using UnityEngine;
using Spotlight.NPC;
using NPCEntity = Spotlight.NPC.NPC;

namespace Spotlight.World2D5
{
    /// <summary>
    /// 2.5D NPC 视图：把 NPC 逻辑绑定到 GameObject，提供世界位置与屏幕位置（供对话映射）。
    /// NPC 移动（INPCMovement）后续接入。
    /// </summary>
    public sealed class NPCView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

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

        private void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }
    }
}

