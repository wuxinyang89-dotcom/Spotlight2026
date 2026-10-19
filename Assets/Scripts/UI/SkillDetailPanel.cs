using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spotlight.Gameplay.Abilities;

namespace Spotlight.UI
{
    /// <summary>
    /// 技能详情面板：列出已拥有的技能（图标 + 名称 + 简介）。数据来自 AbilitySystem。
    /// </summary>
    public sealed class SkillDetailPanel : UIPanel
    {
        public override UILayer Layer => UILayer.Popup;

        private const float RowHeight = 88f;

        private static SkillDetailPanel _instance;

        private RectTransform _content;
        private Text _emptyHint;
        private readonly List<GameObject> _rows = new List<GameObject>();

        /// <summary>打开技能详情面板。</summary>
        public static void Open()
        {
            EnsureInstance();
            _instance.Show();
        }

        private static void EnsureInstance()
        {
            if (_instance != null)
            {
                return;
            }

            UIRoot.Ensure();
            GameObject go = new GameObject("SkillDetailPanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Popup), false);
            _instance = go.AddComponent<SkillDetailPanel>();
        }

        protected override void OnInitialize()
        {
            BuildUI();
        }

        protected override void OnShow()
        {
            Refresh();
        }

        private void BuildUI()
        {
            Image dim = UIFactory.CreateImage("Dim", transform, new Color(0f, 0f, 0f, 0.4f));
            UIFactory.Stretch(dim.rectTransform);

            Image panel = UIFactory.CreateImage("Panel", transform, new Color(0.14f, 0.14f, 0.17f, 1f));
            RectTransform panelRect = panel.rectTransform;
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(720f, 560f);
            panelRect.anchoredPosition = Vector2.zero;

            Text title = UIFactory.CreateText("Title", panel.transform, 26, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleCenter);
            title.text = "技能详情";
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(24f, -52f);
            titleRect.offsetMax = new Vector2(-24f, -12f);

            GameObject contentGo = new GameObject("Content");
            contentGo.transform.SetParent(panel.transform, false);
            RectTransform contentRect = contentGo.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 0f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.offsetMin = new Vector2(24f, 64f);
            contentRect.offsetMax = new Vector2(-24f, -64f);
            _content = contentRect;

            _emptyHint = UIFactory.CreateText("EmptyHint", _content, 20, new Color(0.75f, 0.75f, 0.75f), TextAnchor.MiddleCenter);
            UIFactory.Stretch(_emptyHint.rectTransform);
            _emptyHint.text = "尚未获得技能";

            Button close = UIFactory.CreateButton("Close", panel.transform, "关闭", 20);
            close.onClick.AddListener(Hide);
            RectTransform closeRect = close.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.5f, 0f);
            closeRect.anchorMax = new Vector2(0.5f, 0f);
            closeRect.pivot = new Vector2(0.5f, 0.5f);
            closeRect.anchoredPosition = new Vector2(0f, 20f);
            closeRect.sizeDelta = new Vector2(160f, 48f);
        }

        // ==== 列表刷新 ====

        private void Refresh()
        {
            foreach (GameObject row in _rows)
            {
                if (row != null)
                {
                    Destroy(row);
                }
            }
            _rows.Clear();

            IReadOnlyList<AbilityType> owned = AbilitySystem.Instance.Owned;
            _emptyHint.gameObject.SetActive(owned == null || owned.Count == 0);

            float y = 0f;
            if (owned != null)
            {
                foreach (AbilityType type in owned)
                {
                    AbilityDefinition def = AbilitySystem.Instance.GetDefinition(type);
                    if (def == null)
                    {
                        continue;
                    }

                    _rows.Add(CreateSkillRow(def, y));
                    y += RowHeight;
                }
            }
        }

        private GameObject CreateSkillRow(AbilityDefinition def, float y)
        {
            GameObject row = new GameObject("Skill_" + def.type);
            row.transform.SetParent(_content, false);
            RectTransform rect = row.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(0f, RowHeight);
            rect.anchoredPosition = new Vector2(0f, -y);

            Image icon = UIFactory.CreateImage("Icon", row.transform, Color.white);
            icon.sprite = def.icon != null ? def.icon : UIFactory.CreateCircleSprite(def.orbColor, 64);
            RectTransform iconRect = icon.rectTransform;
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.sizeDelta = new Vector2(64f, 64f);
            iconRect.anchoredPosition = new Vector2(8f, 0f);

            Text name = UIFactory.CreateText("Name", row.transform, 22, Color.white, TextAnchor.MiddleLeft);
            name.text = def.displayName;
            RectTransform nameRect = name.rectTransform;
            nameRect.anchorMin = new Vector2(0f, 1f);
            nameRect.anchorMax = new Vector2(1f, 1f);
            nameRect.offsetMin = new Vector2(88f, -36f);
            nameRect.offsetMax = new Vector2(-12f, -6f);

            Text desc = UIFactory.CreateText("Desc", row.transform, 18, new Color(0.75f, 0.75f, 0.75f), TextAnchor.UpperLeft);
            desc.text = def.description;
            RectTransform descRect = desc.rectTransform;
            descRect.anchorMin = new Vector2(0f, 0f);
            descRect.anchorMax = new Vector2(1f, 1f);
            descRect.offsetMin = new Vector2(88f, 8f);
            descRect.offsetMax = new Vector2(-12f, -40f);

            return row;
        }
    }
}
