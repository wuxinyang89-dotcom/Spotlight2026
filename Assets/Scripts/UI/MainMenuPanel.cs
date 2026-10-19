using UnityEngine;
using UnityEngine.UI;
using Spotlight.Gameplay;
using Spotlight.Gameplay.Save;

namespace Spotlight.UI
{
    /// <summary>
    /// 主界面（Normal 层）：三个按钮——开始游戏、继续游戏、退出游戏。
    /// 继续游戏在无存档时禁用。
    /// </summary>
    public sealed class MainMenuPanel : UIPanel
    {
        public override UILayer Layer => UILayer.Normal;

        private static MainMenuPanel _instance;

        /// <summary>显示主界面。</summary>
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
            GameObject go = new GameObject("MainMenuPanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Normal), false);
            _instance = go.AddComponent<MainMenuPanel>();
        }

        protected override void OnInitialize()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            // 标题
            Text title = UIFactory.CreateText("Title", transform, 44, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleCenter);
            title.text = "Spotlight 2026";
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, -180f);
            titleRect.sizeDelta = new Vector2(800f, 80f);

            // 开始游戏
            Button start = UIFactory.CreateButton("Start", transform, "开始游戏", 26);
            start.onClick.AddListener(SceneFlow.StartNewGame);
            PlaceButton(start, -60f);

            // 继续游戏（无存档则禁用）
            Button continueBtn = UIFactory.CreateButton("Continue", transform, "继续游戏", 26);
            bool hasSave = AutoSaveSystem.Load() != null;
            continueBtn.interactable = hasSave;
            if (hasSave)
            {
                continueBtn.onClick.AddListener(() => SceneFlow.ContinueGame());
            }
            PlaceButton(continueBtn, -140f);

            // 退出游戏
            Button quit = UIFactory.CreateButton("Quit", transform, "退出游戏", 26);
            quit.onClick.AddListener(SceneFlow.Quit);
            PlaceButton(quit, -220f);
        }

        private static void PlaceButton(Button button, float y)
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(360f, 64f);
            rect.anchoredPosition = new Vector2(0f, y);
        }
    }
}
