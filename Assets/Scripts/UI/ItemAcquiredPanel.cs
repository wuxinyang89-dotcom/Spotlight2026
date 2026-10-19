using UnityEngine;
using UnityEngine.UI;
using Spotlight.Data.Inventory;

namespace Spotlight.UI
{
    /// <summary>
    /// 道具获取弹窗：显示道具图标（图案）+ 名称 + 简介，确认后关闭。
    /// 用于传译器等剧情 / 事件道具的获取提示。
    /// </summary>
    public sealed class ItemAcquiredPanel : UIPanel
    {
        public override UILayer Layer => UILayer.Popup;

        private static ItemAcquiredPanel _instance;

        private Image _icon;
        private Text _nameText;
        private Text _descText;
        private ItemDefinition _item;

        /// <summary>弹出道具获取提示。</summary>
        public static void Show(ItemDefinition item)
        {
            EnsureInstance();
            _instance.ShowItem(item);
        }

        private static void EnsureInstance()
        {
            if (_instance != null)
            {
                return;
            }

            UIRoot.Ensure();
            GameObject go = new GameObject("ItemAcquiredPanel");
            go.transform.SetParent(UIRoot.Instance.GetLayerRoot(UILayer.Popup), false);
            _instance = go.AddComponent<ItemAcquiredPanel>();
        }

        private void ShowItem(ItemDefinition item)
        {
            _item = item;
            Show();
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
            // 半透明遮罩
            Image dim = UIFactory.CreateImage("Dim", transform, new Color(0f, 0f, 0f, 0.55f));
            UIFactory.Stretch(dim.rectTransform);

            // 面板
            Image panel = UIFactory.CreateImage("Panel", transform, new Color(0.15f, 0.15f, 0.18f, 1f));
            RectTransform panelRect = panel.rectTransform;
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(560f, 400f);
            panelRect.anchoredPosition = Vector2.zero;

            // 标题
            Text title = UIFactory.CreateText("Title", panel.transform, 26, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleCenter);
            title.text = "获得道具";
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(24f, -52f);
            titleRect.offsetMax = new Vector2(-24f, -12f);

            // 图标
            _icon = UIFactory.CreateImage("Icon", panel.transform, Color.white);
            RectTransform iconRect = _icon.rectTransform;
            iconRect.anchorMin = new Vector2(0.5f, 1f);
            iconRect.anchorMax = new Vector2(0.5f, 1f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = new Vector2(0f, -140f);
            iconRect.sizeDelta = new Vector2(128f, 128f);

            // 名称
            _nameText = UIFactory.CreateText("Name", panel.transform, 24, Color.white, TextAnchor.MiddleCenter);
            RectTransform nameRect = _nameText.rectTransform;
            nameRect.anchorMin = new Vector2(0f, 1f);
            nameRect.anchorMax = new Vector2(1f, 1f);
            nameRect.offsetMin = new Vector2(24f, -216f);
            nameRect.offsetMax = new Vector2(-24f, -180f);

            // 简介
            _descText = UIFactory.CreateText("Desc", panel.transform, 18, new Color(0.8f, 0.8f, 0.8f), TextAnchor.MiddleCenter);
            RectTransform descRect = _descText.rectTransform;
            descRect.anchorMin = new Vector2(0f, 0f);
            descRect.anchorMax = new Vector2(1f, 1f);
            descRect.offsetMin = new Vector2(32f, 80f);
            descRect.offsetMax = new Vector2(-32f, -216f);

            // 确认按钮
            Button close = UIFactory.CreateButton("Close", panel.transform, "确认", 20);
            close.onClick.AddListener(Hide);
            RectTransform closeRect = close.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.5f, 0f);
            closeRect.anchorMax = new Vector2(0.5f, 0f);
            closeRect.pivot = new Vector2(0.5f, 0.5f);
            closeRect.anchoredPosition = new Vector2(0f, 24f);
            closeRect.sizeDelta = new Vector2(200f, 48f);
        }

        private void Refresh()
        {
            if (_item == null)
            {
                return;
            }

            _icon.sprite = _item.icon != null ? _item.icon : UIFactory.CreateCircleSprite(_item.iconColor, 64);
            _nameText.text = _item.displayName;
            _descText.text = _item.description;
        }
    }
}
