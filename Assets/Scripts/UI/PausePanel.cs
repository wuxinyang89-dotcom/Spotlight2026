using UnityEngine;
using UnityEngine.UI;
using Spotlight.Gameplay;

namespace Spotlight.UI
{
    /// <summary>
    /// 暂停界面（Top 层）：三个按钮——查看背包、回到主菜单、继续游戏。
    /// 显示时冻结游戏（Time.timeScale = 0），关闭时恢复。
    /// </summary>
    public sealed class PausePanel : UIPanel
    {
        public override UILayer Layer => UILayer.Top;

        private static PausePanel _instance;

        /// <summary>是否处于暂停中。</summary>
        public static bool IsOpen => _instance != null && _instance.IsVisible;

        /// <summary>打开暂停界面。</summary>
        public static void Open()
        {
            EnsureInstance();
            _instance.Show();
        }

        /// <summary>关闭暂停界面。</summary>
        public static void Close()
        {
            if (_instance != null)
            {
                _instance.Hide();
            }
        }

        /// <summary>切换暂停界面。</summary>
        public static void Toggle()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        private static void EnsureInstance()
        {
            if (_instance != null)
            {
                return;
            }

            UIRoot.Ensure();
            GameObject go = new GameObject("PausePanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Top), false);
            _instance = go.AddComponent<PausePanel>();
        }

        protected override void OnInitialize()
        {
            BuildUI();
        }

        protected override void OnShow()
        {
            Time.timeScale = 0f; // 冻结游戏
        }

        protected override void OnHide()
        {
            Time.timeScale = 1f; // 恢复游戏
        }

        private void BuildUI()
        {
            // 遮罩
            Image dim = UIFactory.CreateImage("Dim", transform, new Color(0f, 0f, 0f, 0.6f));
            UIFactory.Stretch(dim.rectTransform);

            // 面板
            Image panel = UIFactory.CreateImage("Panel", transform, new Color(0.12f, 0.12f, 0.15f, 0.97f));
            RectTransform panelRect = panel.rectTransform;
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(420f, 460f);
            panelRect.anchoredPosition = Vector2.zero;

            // 标题
            Text title = UIFactory.CreateText("Title", panel.transform, 30, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleCenter);
            title.text = "暂停";
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(24f, -72f);
            titleRect.offsetMax = new Vector2(-24f, -16f);

            // 三个按钮
            Button bag = UIFactory.CreateButton("Bag", panel.transform, "查看背包", 22);
            bag.onClick.AddListener(InventoryPanel.Open);
            PlaceButton(bag, 110f);

            Button menu = UIFactory.CreateButton("Menu", panel.transform, "回到主菜单", 22);
            menu.onClick.AddListener(SceneFlow.GoToMainMenu);
            PlaceButton(menu, 38f);

            Button resume = UIFactory.CreateButton("Resume", panel.transform, "继续游戏", 22);
            resume.onClick.AddListener(Hide);
            PlaceButton(resume, -34f);
        }

        private static void PlaceButton(Button button, float y)
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(280f, 56f);
            rect.anchoredPosition = new Vector2(0f, y);
        }
    }
}
