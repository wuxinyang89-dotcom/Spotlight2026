using System;
using UnityEngine;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 能力世界玩家实体：独立于网格、平滑移动的物理对象。
    /// 移动由外部（<see cref="GridPlayerController"/>）以“速度”驱动（非按格），
    /// 触发器用于深坑 / 终点检测。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class GridPlayerAvatar : MonoBehaviour
    {
        [Header("移动速度")]
        [SerializeField, Min(0.01f)] private float moveSpeed = 4f;

        [Header("碰撞体尺寸")]
        [SerializeField] private Vector2 bodySize = new Vector2(0.6f, 0.6f);

        [Header("美术插槽（留空则回退程序化圆形占位）")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Rigidbody2D _body;
        private Vector2 _input;
        private float _levitateRemaining;

        /// <summary>进入某个区域（深坑 / 终点）时触发。</summary>
        public event Action<GridCellZone> ZoneEntered;

        /// <summary>玩家刚体。</summary>
        public Rigidbody2D Body => _body;

        /// <summary>最后移动方向（单位向量；默认朝上，供借力等使用）。</summary>
        public Vector2 Facing { get; private set; } = Vector2.up;

        /// <summary>是否处于悬浮状态（Levity 自身效果）。</summary>
        public bool IsLevitating => _levitateRemaining > 0f;

        /// <summary>世界位置。</summary>
        public Vector3 WorldPosition => transform.position;

        /// <summary>移动速度（由控制器配置）。</summary>
        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(0.01f, value);
        }

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _body.gravityScale = 0f;
            _body.freezeRotation = true;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;

            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.size = bodySize;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = CreatePlaceholderSprite();
            }

            spriteRenderer.sortingOrder = 5;
        }

        /// <summary>设置移动输入（非单位向量会被归一化；零向量停止）。</summary>
        public void SetMoveInput(Vector2 input)
        {
            _input = input.sqrMagnitude > 1f ? input.normalized : input;
            if (_input.sqrMagnitude > 0.0001f)
            {
                Facing = _input;
            }
        }

        /// <summary>开始悬浮（时长秒；非正数无效果）。悬浮期间忽略深坑。</summary>
        public void StartLevitate(float duration)
        {
            if (duration > 0f)
            {
                _levitateRemaining = duration;
            }
        }

        private void Update()
        {
            if (_levitateRemaining > 0f)
            {
                _levitateRemaining -= Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            _body.linearVelocity = _input * moveSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other) => NotifyZone(other);

        private void OnTriggerStay2D(Collider2D other) => NotifyZone(other);

        private void NotifyZone(Collider2D other)
        {
            GridCellZone zone = other.GetComponent<GridCellZone>();
            if (zone != null)
            {
                ZoneEntered?.Invoke(zone);
            }
        }

        private static Sprite CreatePlaceholderSprite()
        {
            const int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = (size - 1) * 0.5f;
            float radius = size * 0.5f - 1f;
            Color color = new Color(0.25f, 0.75f, 1f);
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    pixels[y * size + x] = (dx * dx + dy * dy) <= radius * radius ? color : Color.clear;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }
    }
}
