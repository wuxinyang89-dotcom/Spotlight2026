using UnityEngine;
using UnityEngine.UI;

namespace Spotlight.UI
{
    /// <summary>
    /// 弹窗面板（从简）：通用对话框，包含标题、消息与一个"关闭"按钮。
    /// 属于 Popup 层；同一时间只显示一个弹窗，不做多层级嵌套。
    /// 通过静态方法 Show / ShowError 从任意位置弹出，Close 关闭当前弹窗。
    /// </summary>
    public sealed class PopupDialog : UIPanel
    {
        public override UILayer Layer => UILayer.Popup;

        private static PopupDialog _instance;
        private static Font _defaultFont;

        private Text _titleText;
        private Text _messageText;
        private string _title = string.Empty;
        private string _message = string.Empty;

        /// <summary>显示弹窗。</summary>
        public static void Show(string title, string message)
        {
            EnsureInstance();
            _instance._title = title ?? string.Empty;
            _instance._message = message ?? string.Empty;
            _instance.Show();
        }

        /// <summary>显示报错弹窗（默认标题"运行错误"）。</summary>
        public static void ShowError(string message) => Show("运行错误", message);

        /// <summary>关闭当前弹窗。</summary>
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
            GameObject go = new GameObject("PopupDialog");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Popup), false);
            _instance = go.AddComponent<PopupDialog>();
        }

        protected override void OnInitialize()
        {
            BuildUI();
        }

        protected override void OnShow()
        {
            if (_titleText != null)
            {
                _titleText.text = _title;
            }

            if (_messageText != null)
            {
                _messageText.text = _message;
            }
        }

        private static Font DefaultFont
        {
            get
            {
                if (_defaultFont == null)
                {
                    _defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }

                return _defaultFont;
            }
        }

        private void BuildUI()
        {
            Image dim = CreateImage("Dim", transform, new Color(0f, 0f, 0f, 0.55f));
            Stretch(dim.rectTransform);

            Image panel = CreateImage("Panel", transform, new Color(0.15f, 0.15f, 0.18f, 1f));
            RectTransform panelRect = panel.rectTransform;
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(720f, 320f);
            panelRect.anchoredPosition = Vector2.zero;

            _titleText = CreateText("Title", panel.transform, 28, new Color(1f, 0.45f, 0.45f), TextAnchor.MiddleCenter);
            RectTransform titleRect = _titleText.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(24f, -56f);
            titleRect.offsetMax = new Vector2(-24f, -12f);

            _messageText = CreateText("Message", panel.transform, 22, Color.white, TextAnchor.MiddleCenter);
            RectTransform msgRect = _messageText.rectTransform;
            msgRect.anchorMin = new Vector2(0f, 0f);
            msgRect.anchorMax = new Vector2(1f, 1f);
            msgRect.offsetMin = new Vector2(28f, 76f);
            msgRect.offsetMax = new Vector2(-28f, -72f);

            CreateCloseButton(panel.transform);
        }

        private void CreateCloseButton(Transform parent)
        {
            GameObject go = new GameObject("CloseButton");
            go.transform.SetParent(parent, false);

            Image image = go.AddComponent<Image>();
            image.color = new Color(0.32f, 0.32f, 0.38f, 1f);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(200f, 48f);
            rect.anchoredPosition = new Vector2(0f, 20f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(Hide);

            Text label = CreateText("Label", go.transform, 20, Color.white, TextAnchor.MiddleCenter);
            label.text = "关闭";
            Stretch(label.rectTransform);
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Image image = go.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string name, Transform parent, int fontSize, Color color, TextAnchor alignment)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Text text = go.AddComponent<Text>();
            text.font = DefaultFont;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
