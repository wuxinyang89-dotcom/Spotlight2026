using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Spotlight.Gameplay
{
    /// <summary>
    /// 游戏内运行时错误弹窗：用 UGUI 在屏幕中央显示显式对话框，供玩家查看错误并反馈。
    /// 全局单例（DontDestroyOnLoad），可用静态方法 <see cref="Show(string, string)"/> 从任意位置触发。
    /// </summary>
    public sealed class RuntimeErrorPopup : MonoBehaviour
    {
        private static RuntimeErrorPopup _instance;
        private static Font _defaultFont;

        private Canvas _canvas;
        private Text _titleText;
        private Text _messageText;

        /// <summary>显示错误弹窗（默认标题“运行错误”）。</summary>
        public static void ShowError(string message) => Show("运行错误", message);

        /// <summary>显示错误弹窗。</summary>
        public static void Show(string title, string message)
        {
            EnsureInstance();
            _instance.ShowInternal(title, message);
        }

        /// <summary>关闭错误弹窗。</summary>
        public static void Hide()
        {
            if (_instance != null)
            {
                _instance.HideInternal();
            }
        }

        private static void EnsureInstance()
        {
            if (_instance != null)
            {
                return;
            }

            GameObject go = new GameObject("RuntimeErrorPopup");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<RuntimeErrorPopup>();
            _instance.BuildUI();
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
            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform, false);
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 30000;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasGo.AddComponent<GraphicRaycaster>();

            Image dim = CreateImage("Dim", canvasGo.transform, new Color(0f, 0f, 0f, 0.55f));
            Stretch(dim.rectTransform);

            Image panel = CreateImage("Panel", canvasGo.transform, new Color(0.15f, 0.15f, 0.18f, 1f));
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
            msgRect.offsetMin = new Vector2(28f, 44f);
            msgRect.offsetMax = new Vector2(-28f, -68f);

            Text hint = CreateText("Hint", panel.transform, 16, new Color(0.78f, 0.78f, 0.78f), TextAnchor.MiddleCenter);
            hint.text = "点击任意处或按 Enter / Esc / 空格 关闭，并将此信息反馈给开发者";
            RectTransform hintRect = hint.rectTransform;
            hintRect.anchorMin = new Vector2(0f, 0f);
            hintRect.anchorMax = new Vector2(1f, 0f);
            hintRect.offsetMin = new Vector2(24f, 10f);
            hintRect.offsetMax = new Vector2(-24f, 34f);

            _canvas.gameObject.SetActive(false);
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

        private void ShowInternal(string title, string message)
        {
            if (_titleText != null)
            {
                _titleText.text = title;
            }

            if (_messageText != null)
            {
                _messageText.text = message;
            }

            _canvas.gameObject.SetActive(true);
        }

        private void HideInternal()
        {
            if (_canvas != null)
            {
                _canvas.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (_canvas == null || !_canvas.gameObject.activeSelf)
            {
                return;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                HideInternal();
                return;
            }

            if (Keyboard.current != null)
            {
                if (Keyboard.current.enterKey.wasPressedThisFrame
                    || Keyboard.current.escapeKey.wasPressedThisFrame
                    || Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    HideInternal();
                }
            }
        }
    }
}
