using System.Collections.Generic;
using UnityEngine;
using Spotlight.NPC.Appearance;
using NPCEntity = Spotlight.NPC.NPC;

namespace Spotlight.World2D5
{
    /// <summary>
    /// 2.5D NPC 视图：绑定 NPC 逻辑、提供世界/屏幕位置（供对话映射），
    /// 并按身体部件（纸娃娃）渲染外观——种族属性外化在外貌上。
    /// 外观来源优先级：Inspector 指定 > NPC 种族属性的 Appearance > 无（仅显示占位身体）。
    /// </summary>
    public sealed class NPCView : MonoBehaviour
    {
        [Header("外观（可选：Inspector 直接指定；否则从种族属性读取）")]
        [SerializeField] private NPCAppearanceAsset appearance;

        [Header("人物碰撞体（与图片尺寸相适应）")]
        [SerializeField] private Vector2 bodySize = new Vector2(0.6f, 0.8f);

        [Header("部件渲染基准排序层")]
        [SerializeField] private int baseSortingOrder = 1;

        private NPCEntity _npc;
        private readonly List<SpriteRenderer> _parts = new List<SpriteRenderer>();

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

        /// <summary>绑定 NPC，并从其种族属性读取外观。</summary>
        public void Initialize(NPCEntity npc)
        {
            _npc = npc;

            NPCAppearanceAsset app = appearance;
            if (app == null && npc != null && npc.Race != null)
            {
                app = npc.Race.Appearance;
            }

            ApplyAppearance(app);
        }

        /// <summary>按身体部件（纸娃娃）应用外观；传 null 则清空所有部件。</summary>
        public void ApplyAppearance(NPCAppearanceAsset app)
        {
            ClearParts();
            if (app == null)
            {
                return;
            }

            for (int i = 0; i < NPCAppearanceAsset.RenderOrder.Length; i++)
            {
                NPCBodyPartType type = NPCAppearanceAsset.RenderOrder[i];
                NPCPartDefinition part = app.GetPart(type);

                if (type == NPCBodyPartType.Body)
                {
                    // 身体：肤色外化；无贴图时用肤色占位，保证可见。
                    CreatePart(type, part, app.skinColor, baseSortingOrder + i, useFallback: true);
                }
                else if (part != null && part.sprite != null)
                {
                    // 头部部件：仅有贴图时渲染（美术补齐后自动显示）。
                    CreatePart(type, part, part.color, baseSortingOrder + i, useFallback: false);
                }
            }
        }

        private void CreatePart(NPCBodyPartType type, NPCPartDefinition part, Color color, int sortingOrder, bool useFallback)
        {
            GameObject go = new GameObject(type.ToString());
            go.transform.SetParent(transform, false);

            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = part != null ? part.sprite : null;

            if (renderer.sprite == null && useFallback)
            {
                renderer.sprite = PlaceholderSprite;
            }

            if (renderer.sprite == null)
            {
                Destroy(go); // 无贴图且无占位，跳过该部件。
                return;
            }

            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            _parts.Add(renderer);
        }

        private void ClearParts()
        {
            foreach (SpriteRenderer renderer in _parts)
            {
                if (renderer != null)
                {
                    Destroy(renderer.gameObject);
                }
            }

            _parts.Clear();
        }

        private void Awake()
        {
            SetupCollider();
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

        private static Sprite _placeholderSprite;

        /// <summary>白色圆形占位（肤色通过 SpriteRenderer.color 着色）。</summary>
        private static Sprite PlaceholderSprite
        {
            get
            {
                if (_placeholderSprite == null)
                {
                    const int size = 32;
                    Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                    float center = (size - 1) * 0.5f;
                    float radius = size * 0.5f - 1f;
                    Color[] pixels = new Color[size * size];

                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            float dx = x - center;
                            float dy = y - center;
                            pixels[y * size + x] = (dx * dx + dy * dy) <= radius * radius ? Color.white : Color.clear;
                        }
                    }

                    tex.SetPixels(pixels);
                    tex.Apply();
                    _placeholderSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
                }

                return _placeholderSprite;
            }
        }
    }
}



