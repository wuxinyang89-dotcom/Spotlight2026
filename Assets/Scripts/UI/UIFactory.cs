using UnityEngine;
using UnityEngine.UI;

namespace Spotlight.UI
{
    /// <summary>
    /// UGUI 代码构建工具：集中提供字体、Text / Image / Button 与程序化 Sprite（圆形 / 圆环）的创建，
    /// 供各 UI 面板在 OnInitialize 中构建界面使用，避免重复代码。
    /// </summary>
    public static class UIFactory
    {
        private static Font _defaultFont;

        /// <summary>默认字体（内置 LegacyRuntime）。</summary>
        public static Font DefaultFont
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

        /// <summary>创建文本（默认不参与点击，避免遮挡按钮）。</summary>
        public static Text CreateText(string name, Transform parent, int fontSize, Color color, TextAnchor alignment)
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
            text.raycastTarget = false;
            return text;
        }

        /// <summary>创建纯色图片。</summary>
        public static Image CreateImage(string name, Transform parent, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Image image = go.AddComponent<Image>();
            image.color = color;
            return image;
        }

        /// <summary>创建带文字的按钮（点击回调由调用方 AddListener）。</summary>
        public static Button CreateButton(string name, Transform parent, string label, int fontSize = 20)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);

            Image image = go.AddComponent<Image>();
            image.color = new Color(0.32f, 0.32f, 0.38f, 1f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = image;

            Text text = CreateText("Label", go.transform, fontSize, Color.white, TextAnchor.MiddleCenter);
            text.text = label;
            Stretch(text.rectTransform);

            return button;
        }

        /// <summary>把 RectTransform 铺满父节点。</summary>
        public static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        /// <summary>生成圆形 Sprite（实心圆，用于技能光球等）。</summary>
        public static Sprite CreateCircleSprite(Color color, int size = 64)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = (size - 1) * 0.5f;
            float radius = size * 0.5f - 1f;
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    pixels[y * size + x] = dist <= radius ? color : Color.clear;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }

        /// <summary>生成圆环 Sprite（环带，用于技能圆环背景）。</summary>
        public static Sprite CreateRingSprite(Color color, int size = 256, float thicknessRatio = 0.12f)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = (size - 1) * 0.5f;
            float outerRadius = size * 0.5f - 1f;
            float innerRadius = outerRadius * (1f - thicknessRatio);
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    bool insideRing = dist <= outerRadius && dist >= innerRadius;
                    pixels[y * size + x] = insideRing ? color : Color.clear;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }
    }
}
