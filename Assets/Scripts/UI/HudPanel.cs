using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spotlight.UI
{
    /// <summary>
    /// 常驻 HUD 面板（Normal 层）：包含
    /// 1) 左上角任务列表（点击打开任务日志）；
    /// 2) 右下角背包图标（点击打开背包详情）；
    /// 3) 左下角技能圆环（光球数量按拥有能力球数量动态设置，点击查看技能详情）。
    /// 各"打开详情"目前均为空壳，具体界面后续实现。
    /// </summary>
    public sealed class HudPanel : UIPanel
    {
        public override UILayer Layer => UILayer.Normal;

        private const float RingDiameter = 160f;
        private const float OrbSize = 44f;
        private const float RingMargin = 24f;

        private static HudPanel _instance;

        private RectTransform _ringRect;
        private readonly List<GameObject> _orbObjects = new List<GameObject>();

        private static readonly Color[] OrbPalette = new Color[]
        {
            new Color(1f, 0.35f, 0.35f),
            new Color(0.4f, 0.85f, 0.4f),
            new Color(0.35f, 0.55f, 1f),
            new Color(1f, 0.85f, 0.3f),
            new Color(0.75f, 0.45f, 1f),
            new Color(0.35f, 0.9f, 0.85f),
        };

        /// <summary>显示常驻 HUD（首次调用时自动创建）。</summary>
        public static void ShowHud()
        {
            EnsureInstance();
            _instance.Show();
        }

        /// <summary>隐藏常驻 HUD。</summary>
        public static void HideHud()
        {
            if (_instance != null)
            {
                _instance.Hide();
            }
        }

        /// <summary>根据拥有能力球数量刷新技能圆环上的光球（供道具系统调用）。</summary>
        public static void SetOrbCount(int count)
        {
            if (_instance != null)
            {
                _instance.RefreshOrbs(count);
            }
        }

        private static void EnsureInstance()
        {
            if (_instance != null)
            {
                return;
            }

            UIRoot.Ensure();
            GameObject go = new GameObject("HudPanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Normal), false);
            _instance = go.AddComponent<HudPanel>();
        }

        protected override void OnInitialize()
        {
            BuildTaskList();
            BuildInventoryIcon();
            BuildSkillRing();
        }

        // ---------- 1. 左上角任务列表 ----------

        private void BuildTaskList()
        {
            Text title = UIFactory.CreateText("TaskTitle", transform, 22, new Color(1f, 0.85f, 0.4f), TextAnchor.UpperLeft);
            title.text = "任务";
            RectTransform tRect = title.rectTransform;
            tRect.anchorMin = new Vector2(0f, 1f);
            tRect.anchorMax = new Vector2(0f, 1f);
            tRect.pivot = new Vector2(0f, 1f);
            tRect.anchoredPosition = new Vector2(24f, -24f);
            tRect.sizeDelta = new Vector2(200f, 32f);

            // 占位任务条目（可点击打开任务日志）
            Button taskButton = UIFactory.CreateButton("TaskItem", transform, "示例任务：调查神秘信号", 18);
            taskButton.onClick.AddListener(TaskLogPanel.Open);
            RectTransform bRect = taskButton.GetComponent<RectTransform>();
            bRect.anchorMin = new Vector2(0f, 1f);
            bRect.anchorMax = new Vector2(0f, 1f);
            bRect.pivot = new Vector2(0f, 1f);
            bRect.anchoredPosition = new Vector2(24f, -64f);
            bRect.sizeDelta = new Vector2(320f, 40f);
        }

        // ---------- 2. 右下角背包图标 ----------

        private void BuildInventoryIcon()
        {
            Button bagButton = UIFactory.CreateButton("InventoryIcon", transform, "背包", 20);
            bagButton.onClick.AddListener(InventoryPanel.Open);
            RectTransform bRect = bagButton.GetComponent<RectTransform>();
            bRect.anchorMin = new Vector2(1f, 0f);
            bRect.anchorMax = new Vector2(1f, 0f);
            bRect.pivot = new Vector2(1f, 0f);
            bRect.anchoredPosition = new Vector2(-24f, 24f);
            bRect.sizeDelta = new Vector2(96f, 96f);
        }

        // ---------- 3. 技能圆环（左下角） ----------

        private void BuildSkillRing()
        {
            Color ringColor = new Color(0.4f, 0.45f, 0.55f, 0.9f);

            Image ring = UIFactory.CreateImage("SkillRing", transform, Color.white);
            ring.sprite = UIFactory.CreateRingSprite(ringColor);
            ring.raycastTarget = false;
            _ringRect = ring.rectTransform;
            _ringRect.anchorMin = new Vector2(0f, 0f);
            _ringRect.anchorMax = new Vector2(0f, 0f);
            _ringRect.pivot = new Vector2(0.5f, 0.5f);
            _ringRect.anchoredPosition = new Vector2(RingDiameter * 0.5f + RingMargin, RingDiameter * 0.5f + RingMargin);
            _ringRect.sizeDelta = new Vector2(RingDiameter, RingDiameter);

            // 占位：默认 4 个能力球，后续由道具系统调用 SetOrbCount 驱动
            RefreshOrbs(4);
        }

        /// <summary>按数量重建圆环上的光球（均匀分布；0 或负数表示清空）。</summary>
        private void RefreshOrbs(int count)
        {
            if (_ringRect == null)
            {
                return;
            }

            foreach (GameObject orb in _orbObjects)
            {
                if (orb != null)
                {
                    Destroy(orb);
                }
            }
            _orbObjects.Clear();

            if (count <= 0)
            {
                return;
            }

            float orbRadius = RingDiameter * 0.5f - OrbSize * 0.5f + 4f;
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                Button orb = UIFactory.CreateButton("SkillOrb", _ringRect, string.Empty, 16);
                Image orbImage = orb.GetComponent<Image>();
                orbImage.sprite = UIFactory.CreateCircleSprite(OrbPalette[i % OrbPalette.Length]);
                orbImage.color = Color.white;
                orb.onClick.AddListener(SkillDetailPanel.Open);

                RectTransform orbRect = orb.GetComponent<RectTransform>();
                orbRect.anchorMin = new Vector2(0.5f, 0.5f);
                orbRect.anchorMax = new Vector2(0.5f, 0.5f);
                orbRect.pivot = new Vector2(0.5f, 0.5f);
                orbRect.anchoredPosition = dir * orbRadius;
                orbRect.sizeDelta = new Vector2(OrbSize, OrbSize);

                _orbObjects.Add(orb.gameObject);
            }
        }
    }
}
