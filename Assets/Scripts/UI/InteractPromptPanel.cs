using UnityEngine;
using UnityEngine.UI;

namespace Spotlight.UI
{
    /// <summary>
    /// 可交互提示面板（统一弹窗系统）：玩家靠近可交互目标时显示提示（如"按 F 对话"）。
    /// </summary>
    public sealed class InteractPromptPanel : UIPanel
    {
        public override UILayer Layer => UILayer.Normal;

        private static InteractPromptPanel _instance;

        private Text _promptText;

        /// <summary>显示提示。</summary>
        public static void ShowPrompt(string text)
        {
            EnsureInstance();
            _instance.SetPrompt(text);
            _instance.Show();
        }

        /// <summary>隐藏提示。</summary>
        public static void HidePrompt()
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
            GameObject go = new GameObject("InteractPromptPanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Normal), false);
            _instance = go.AddComponent<InteractPromptPanel>();
        }

        protected override void OnInitialize()
        {
            Image bg = UIFactory.CreateImage("Bg", transform, new Color(0f, 0f, 0f, 0.6f));
            RectTransform bgRect = bg.rectTransform;
            bgRect.anchorMin = new Vector2(0.5f, 0f);
            bgRect.anchorMax = new Vector2(0.5f, 0f);
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            bgRect.anchoredPosition = new Vector2(0f, 320f);
            bgRect.sizeDelta = new Vector2(260f, 48f);

            _promptText = UIFactory.CreateText("Prompt", bg.transform, 20, Color.white, TextAnchor.MiddleCenter);
            UIFactory.Stretch(_promptText.rectTransform);
        }

        private void SetPrompt(string text)
        {
            if (_promptText != null)
            {
                _promptText.text = text;
            }
        }
    }
}
