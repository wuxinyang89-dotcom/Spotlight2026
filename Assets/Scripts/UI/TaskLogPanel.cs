using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spotlight.Data.Quest;

namespace Spotlight.UI
{
    /// <summary>
    /// 任务日志面板：列出当前激活的任务（标题 + 描述 + 目标）。数据来自 QuestSystem。
    /// </summary>
    public sealed class TaskLogPanel : UIPanel
    {
        public override UILayer Layer => UILayer.Popup;

        private static TaskLogPanel _instance;

        private RectTransform _content;
        private Text _emptyHint;
        private readonly List<GameObject> _rows = new List<GameObject>();

        /// <summary>打开任务日志面板。</summary>
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
            GameObject go = new GameObject("TaskLogPanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Popup), false);
            _instance = go.AddComponent<TaskLogPanel>();
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
            title.text = "任务日志";
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
            _emptyHint.text = "暂无任务";

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

            IReadOnlyList<QuestDefinition> active = QuestSystem.Instance.Active;
            _emptyHint.gameObject.SetActive(active == null || active.Count == 0);

            float y = 0f;
            if (active != null)
            {
                foreach (QuestDefinition quest in active)
                {
                    _rows.Add(CreateQuestRow(quest, y));
                    y += 128f;
                }
            }
        }

        private GameObject CreateQuestRow(QuestDefinition quest, float y)
        {
            GameObject row = new GameObject("Quest_" + quest.id);
            row.transform.SetParent(_content, false);
            RectTransform rect = row.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(0f, 128f);
            rect.anchoredPosition = new Vector2(0f, -y);

            Text title = UIFactory.CreateText("Title", row.transform, 22, new Color(1f, 0.85f, 0.4f), TextAnchor.UpperLeft);
            title.text = quest.title;
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(8f, -30f);
            titleRect.offsetMax = new Vector2(-8f, -4f);

            Text objective = UIFactory.CreateText("Objective", row.transform, 18, Color.white, TextAnchor.UpperLeft);
            objective.text = "目标：" + quest.objective;
            RectTransform objRect = objective.rectTransform;
            objRect.anchorMin = new Vector2(0f, 1f);
            objRect.anchorMax = new Vector2(1f, 1f);
            objRect.offsetMin = new Vector2(8f, -60f);
            objRect.offsetMax = new Vector2(-8f, -34f);

            Text desc = UIFactory.CreateText("Desc", row.transform, 16, new Color(0.75f, 0.75f, 0.75f), TextAnchor.UpperLeft);
            desc.text = quest.description;
            RectTransform descRect = desc.rectTransform;
            descRect.anchorMin = new Vector2(0f, 0f);
            descRect.anchorMax = new Vector2(1f, 1f);
            descRect.offsetMin = new Vector2(8f, 6f);
            descRect.offsetMax = new Vector2(-8f, -66f);

            return row;
        }
    }
}
