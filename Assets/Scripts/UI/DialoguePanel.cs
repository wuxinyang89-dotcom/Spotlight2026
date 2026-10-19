using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Spotlight.Data.Dialogue;

namespace Spotlight.UI
{
    /// <summary>
    /// 对话面板（弹窗式，与弹窗系统统一）：逐条播放对话台词，包含说话人、台词与继续提示图标。
    /// 点击继续图标（或按 Enter / 空格）播放下一句，对话结束自动关闭。
    /// 弹窗位置与说话者位置映射：说话者在上半屏时弹窗位于下方，反之位于上方（人物位置后补，当前默认下方）。
    /// </summary>
    public sealed class DialoguePanel : UIPanel
    {
        public override UILayer Layer => UILayer.Popup;

        private static DialoguePanel _instance;

        private Text _speakerText;
        private Text _lineText;
        private RectTransform _panelRect;

        private DialogueAsset _dialogue;
        private Func<string, Vector2> _speakerResolver;
        private Action _onComplete;
        private int _lineIndex;

        /// <summary>开始播放一段对话（可传入"说话者名字 → 屏幕位置"解析器用于位置映射）。</summary>
        public static void Play(DialogueAsset dialogue, Func<string, Vector2> speakerScreenPositionResolver = null, Action onComplete = null)
        {
            EnsureInstance();
            _instance.StartDialogue(dialogue, speakerScreenPositionResolver, onComplete);
        }

        /// <summary>关闭当前对话。</summary>
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
            GameObject go = new GameObject("DialoguePanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Popup), false);
            _instance = go.AddComponent<DialoguePanel>();
        }

        protected override void OnInitialize()
        {
            BuildUI();
        }

        private void StartDialogue(DialogueAsset dialogue, Func<string, Vector2> speakerResolver, Action onComplete)
        {
            _dialogue = dialogue;
            _speakerResolver = speakerResolver;
            _onComplete = onComplete;
            _lineIndex = 0;

            if (_dialogue == null || _dialogue.Lines.Count == 0)
            {
                _onComplete?.Invoke();
                return;
            }

            Show();
            ShowCurrentLine();
        }

        private void ShowCurrentLine()
        {
            if (_dialogue == null || _lineIndex < 0 || _lineIndex >= _dialogue.Lines.Count)
            {
                return;
            }

            DialogueLine line = _dialogue.Lines[_lineIndex];
            if (_speakerText != null)
            {
                _speakerText.text = string.IsNullOrEmpty(line.speaker) ? "？？？" : line.speaker;
            }

            if (_lineText != null)
            {
                _lineText.text = line.text ?? string.Empty;
            }

            // 位置映射：按当前说话者解析屏幕位置（支持多说话者 / 旁观对话）
            if (_speakerResolver != null)
            {
                Vector2 speakerPos = _speakerResolver(line.speaker);
                ApplySpeakerPosition(speakerPos);
            }
        }

        private void Advance()
        {
            if (_dialogue == null)
            {
                Hide();
                _onComplete?.Invoke();
                _onComplete = null;
                return;
            }

            _lineIndex++;
            if (_lineIndex >= _dialogue.Lines.Count)
            {
                Hide();
                _onComplete?.Invoke();
                _onComplete = null;
                return;
            }

            ShowCurrentLine();
        }

        private void Update()
        {
            if (!IsVisible)
            {
                return;
            }

            if (Keyboard.current != null
                && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
            {
                Advance();
            }
        }

        private void ApplySpeakerPosition(Vector2 screenPosition01)
        {
            // 说话者在上半屏 → 弹窗在下；反之在上。
            SetPanelAtBottom(screenPosition01.y >= 0.5f);
        }

        private void SetPanelAtBottom(bool atBottom)
        {
            if (_panelRect == null)
            {
                return;
            }

            _panelRect.anchorMin = new Vector2(0.5f, atBottom ? 0f : 1f);
            _panelRect.anchorMax = new Vector2(0.5f, atBottom ? 0f : 1f);
            _panelRect.pivot = new Vector2(0.5f, 0.5f);
            _panelRect.anchoredPosition = new Vector2(0f, atBottom ? 140f : -140f);
        }

        private void BuildUI()
        {
            // 半透明遮罩（聚焦对话）
            Image dim = UIFactory.CreateImage("Dim", transform, new Color(0f, 0f, 0f, 0.2f));
            UIFactory.Stretch(dim.rectTransform);

            // 对话面板（弹窗式）
            Image panel = UIFactory.CreateImage("Panel", transform, new Color(0.12f, 0.12f, 0.15f, 0.95f));
            _panelRect = panel.rectTransform;
            _panelRect.sizeDelta = new Vector2(720f, 180f);
            SetPanelAtBottom(true);

            // 说话人
            _speakerText = UIFactory.CreateText("Speaker", panel.transform, 24, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleLeft);
            RectTransform speakerRect = _speakerText.rectTransform;
            speakerRect.anchorMin = new Vector2(0f, 1f);
            speakerRect.anchorMax = new Vector2(1f, 1f);
            speakerRect.offsetMin = new Vector2(24f, -40f);
            speakerRect.offsetMax = new Vector2(-24f, -8f);

            // 台词
            _lineText = UIFactory.CreateText("Line", panel.transform, 20, Color.white, TextAnchor.MiddleLeft);
            RectTransform lineRect = _lineText.rectTransform;
            lineRect.anchorMin = new Vector2(0f, 0f);
            lineRect.anchorMax = new Vector2(1f, 1f);
            lineRect.offsetMin = new Vector2(24f, 16f);
            lineRect.offsetMax = new Vector2(-64f, -56f);

            // 继续提示图标（右下角 ▼）
            Button continueButton = UIFactory.CreateButton("Continue", panel.transform, "▼", 20);
            continueButton.onClick.AddListener(Advance);
            RectTransform continueRect = continueButton.GetComponent<RectTransform>();
            continueRect.anchorMin = new Vector2(1f, 0f);
            continueRect.anchorMax = new Vector2(1f, 0f);
            continueRect.pivot = new Vector2(1f, 0f);
            continueRect.anchoredPosition = new Vector2(-16f, 16f);
            continueRect.sizeDelta = new Vector2(44f, 44f);
        }
    }
}
