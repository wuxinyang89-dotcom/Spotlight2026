using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Spotlight.Data.CG;

namespace Spotlight.UI
{
    /// <summary>
    /// CG 播放器（Top 层）：逐格播放静态漫画分镜（图片 + 字幕）。
    /// 点击 / 按 Enter / 空格推进；结束调用回调。
    /// </summary>
    public sealed class CGPlayer : UIPanel
    {
        public override UILayer Layer => UILayer.Top;

        private static CGPlayer _instance;
        private static CGAsset _pendingAsset;
        private static Action _pendingCallback;

        private Image _image;
        private Text _subtitle;
        private int _index;

        /// <summary>播放一段 CG。</summary>
        public static void Play(CGAsset asset, Action onComplete = null)
        {
            EnsureInstance();
            _pendingAsset = asset;
            _pendingCallback = onComplete;
            _instance.Show();
        }

        private static void EnsureInstance()
        {
            if (_instance != null)
            {
                return;
            }

            UIRoot.Ensure();
            GameObject go = new GameObject("CGPlayer");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Top), false);
            _instance = go.AddComponent<CGPlayer>();
        }

        protected override void OnInitialize()
        {
            Image bg = UIFactory.CreateImage("Bg", transform, Color.black);
            UIFactory.Stretch(bg.rectTransform);

            _image = UIFactory.CreateImage("Image", transform, Color.white);
            RectTransform imgRect = _image.rectTransform;
            imgRect.anchorMin = Vector2.zero;
            imgRect.anchorMax = Vector2.one;
            imgRect.offsetMin = new Vector2(0f, 140f);
            imgRect.offsetMax = new Vector2(0f, -140f);

            _subtitle = UIFactory.CreateText("Subtitle", transform, 24, Color.white, TextAnchor.MiddleCenter);
            RectTransform subRect = _subtitle.rectTransform;
            subRect.anchorMin = new Vector2(0f, 0f);
            subRect.anchorMax = new Vector2(1f, 0f);
            subRect.pivot = new Vector2(0.5f, 0.5f);
            subRect.anchoredPosition = new Vector2(0f, 90f);
            subRect.sizeDelta = new Vector2(0f, 80f);
        }

        protected override void OnShow()
        {
            _index = 0;
            ShowCurrent();
        }

        private void Update()
        {
            if (!IsVisible)
            {
                return;
            }

            bool advance = false;
            if (Keyboard.current != null)
            {
                advance = Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                advance = true;
            }

            if (advance)
            {
                Advance();
            }
        }

        private void Advance()
        {
            _index++;

            CGAsset asset = _pendingAsset;
            if (asset == null || _index >= asset.Segments.Count)
            {
                Hide();
                Action callback = _pendingCallback;
                _pendingCallback = null;
                callback?.Invoke();
                return;
            }

            ShowCurrent();
        }

        private void ShowCurrent()
        {
            CGAsset asset = _pendingAsset;
            if (asset == null || _index < 0 || _index >= asset.Segments.Count)
            {
                return;
            }

            CGSegment segment = asset.Segments[_index];

            if (_image != null)
            {
                _image.sprite = segment.image;
                _image.color = segment.image != null ? Color.white : Color.clear;
            }

            if (_subtitle != null)
            {
                _subtitle.text = segment.subtitle ?? string.Empty;
            }
        }
    }
}
