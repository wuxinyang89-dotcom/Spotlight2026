using UnityEngine;
using UnityEngine.UI;

namespace Spotlight.UI
{
    /// <summary>
    /// 教程提示面板：屏幕中间显示一段教程文本（无标题、轻量）。
    /// 用于能力世界教学关的步骤提示。
    /// </summary>
    public sealed class TutorialPanel : UIPanel
    {
        public override UILayer Layer => UILayer.Popup;

        private static TutorialPanel _instance;
        private static string _pendingText;

        private Text _text;

        /// <summary>显示一段教程提示。</summary>
        public static void Show(string text)
        {
            EnsureInstance();
            _pendingText = text ?? string.Empty;
            _instance.Show();
        }

        /// <summary>隐藏教程提示。</summary>
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
            GameObject go = new GameObject("TutorialPanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Popup), false);
            _instance = go.AddComponent<TutorialPanel>();
        }

        protected override void OnInitialize()
        {
            Image bg = UIFactory.CreateImage("Bg", transform, new Color(0f, 0f, 0f, 0.7f));
            RectTransform bgRect = bg.rectTransform;
            bgRect.anchorMin = new Vector2(0.5f, 0.5f);
            bgRect.anchorMax = new Vector2(0.5f, 0.5f);
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            bgRect.sizeDelta = new Vector2(760f, 120f);
            bgRect.anchoredPosition = Vector2.zero;

            _text = UIFactory.CreateText("Text", bg.transform, 22, Color.white, TextAnchor.MiddleCenter);
            UIFactory.Stretch(_text.rectTransform);
        }

        protected override void OnShow()
        {
            if (_text != null)
            {
                _text.text = _pendingText;
            }
        }
    }
}
