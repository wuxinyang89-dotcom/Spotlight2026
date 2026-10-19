using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spotlight.UI
{
    /// <summary>
    /// 选项面板（Top 层）：显示一个问题 + 若干选项按钮，点击回调并关闭。
    /// 用于「共鸣 / 暂缓」等剧情分支。
    /// </summary>
    public sealed class ChoicePanel : UIPanel
    {
        public override UILayer Layer => UILayer.Top;

        private static ChoicePanel _instance;
        private static string _pendingQuestion;
        private static IReadOnlyList<string> _pendingOptions;
        private static Action<int> _pendingCallback;

        private Text _questionText;
        private Transform _panel;
        private readonly List<Button> _buttons = new List<Button>();

        /// <summary>显示选项面板。</summary>
        public static void Show(string question, IReadOnlyList<string> options, Action<int> onSelected)
        {
            EnsureInstance();
            _pendingQuestion = question;
            _pendingOptions = options;
            _pendingCallback = onSelected;
            _instance.Show();
        }

        /// <summary>关闭选项面板。</summary>
        public static void Close()
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
            GameObject go = new GameObject("ChoicePanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Top), false);
            _instance = go.AddComponent<ChoicePanel>();
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
            Image dim = UIFactory.CreateImage("Dim", transform, new Color(0f, 0f, 0f, 0.5f));
            UIFactory.Stretch(dim.rectTransform);

            Image panel = UIFactory.CreateImage("Panel", transform, new Color(0.12f, 0.12f, 0.15f, 0.97f));
            RectTransform panelRect = panel.rectTransform;
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(560f, 420f);
            panelRect.anchoredPosition = Vector2.zero;
            _panel = panel.transform;

            _questionText = UIFactory.CreateText("Question", _panel, 24, Color.white, TextAnchor.MiddleCenter);
            RectTransform qRect = _questionText.rectTransform;
            qRect.anchorMin = new Vector2(0f, 1f);
            qRect.anchorMax = new Vector2(1f, 1f);
            qRect.offsetMin = new Vector2(32f, -160f);
            qRect.offsetMax = new Vector2(-32f, -40f);
        }

        private void Refresh()
        {
            foreach (Button b in _buttons)
            {
                if (b != null)
                {
                    Destroy(b.gameObject);
                }
            }
            _buttons.Clear();

            _questionText.text = _pendingQuestion ?? string.Empty;

            if (_pendingOptions == null)
            {
                return;
            }

            for (int i = 0; i < _pendingOptions.Count; i++)
            {
                int index = i;
                Button btn = UIFactory.CreateButton("Option_" + i, _panel, _pendingOptions[i], 22);
                btn.onClick.AddListener(() =>
                {
                    Action<int> callback = _pendingCallback;
                    Hide();
                    callback?.Invoke(index);
                });

                RectTransform rect = btn.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(300f, 56f);
                rect.anchoredPosition = new Vector2(0f, 60f - i * 76f);

                _buttons.Add(btn);
            }
        }
    }
}
