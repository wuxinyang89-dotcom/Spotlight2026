using UnityEngine;
using UnityEngine.UI;

namespace Spotlight.UI
{
    /// <summary>
    /// 常驻 HUD 面板（Normal 层）：包含
    /// 1) 左上角任务列表（点击打开任务日志）；
    /// 2) 右下角背包图标（点击打开背包详情）；
    /// 3) 中下方技能圆环（四个彩色光球均匀分布，点击查看技能详情）。
    /// 各"打开详情"目前均为空壳，具体界面后续实现。
    /// </summary>
    public sealed class HudPanel : UIPanel
    {
        public override UILayer Layer => UILayer.Normal;

        private static HudPanel _instance;

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

        // ---------- 3. 技能圆环 ----------

        private void BuildSkillRing()
        {
            const float ringDiameter = 280f;
            const float orbRadius = 132f;
            const float orbSize = 64f;

            Color ringColor = new Color(0.4f, 0.45f, 0.55f, 0.9f);
            Color[] orbColors = new Color[]
            {
                new Color(1f, 0.35f, 0.35f),
                new Color(0.4f, 0.85f, 0.4f),
                new Color(0.35f, 0.55f, 1f),
                new Color(1f, 0.85f, 0.3f),
            };

            // 圆环（预留 Sprite）
            Image ring = UIFactory.CreateImage("SkillRing", transform, Color.white);
            ring.sprite = UIFactory.CreateRingSprite(ringColor);
            ring.raycastTarget = false;
            RectTransform ringRect = ring.rectTransform;
            ringRect.anchorMin = new Vector2(0.5f, 0f);
            ringRect.anchorMax = new Vector2(0.5f, 0f);
            ringRect.pivot = new Vector2(0.5f, 0.5f);
            ringRect.anchoredPosition = new Vector2(0f, 220f);
            ringRect.sizeDelta = new Vector2(ringDiameter, ringDiameter);

            // 四个彩色光球均匀分布（对角 45°/135°/225°/315°），点击打开技能详情
            for (int i = 0; i < orbColors.Length; i++)
            {
                float angle = (45f + i * 90f) * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                Button orb = UIFactory.CreateButton("SkillOrb", ring.transform, string.Empty, 16);
                Image orbImage = orb.GetComponent<Image>();
                orbImage.sprite = UIFactory.CreateCircleSprite(orbColors[i]);
                orbImage.color = Color.white;
                orb.onClick.AddListener(SkillDetailPanel.Open);

                RectTransform orbRect = orb.GetComponent<RectTransform>();
                orbRect.anchorMin = new Vector2(0.5f, 0.5f);
                orbRect.anchorMax = new Vector2(0.5f, 0.5f);
                orbRect.pivot = new Vector2(0.5f, 0.5f);
                orbRect.anchoredPosition = dir * orbRadius;
                orbRect.sizeDelta = new Vector2(orbSize, orbSize);
            }
        }
    }
}
