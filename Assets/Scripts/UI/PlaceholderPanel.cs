using UnityEngine;
using UnityEngine.UI;

namespace Spotlight.UI
{
    /// <summary>
    /// 占位详情面板基类：仅包含标题、占位提示与关闭按钮，作为"点击入口后打开的空壳界面"。
    /// 具体界面内容后续在子类中填充（重写 OnInitialize 或替换整体构建）。
    /// </summary>
    public abstract class PlaceholderPanel : UIPanel
    {
        public override UILayer Layer => UILayer.Popup;

        /// <summary>面板标题。</summary>
        protected abstract string PanelTitle { get; }

        /// <summary>打开（或创建）指定类型面板并显示。</summary>
        protected static void OpenPanel<T>() where T : PlaceholderPanel
        {
            UIRoot.Ensure();

            if (UIManager.Instance.Get<T>() == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                go.AddComponent<T>();
            }

            UIManager.Instance.Open<T>();
        }

        protected override void OnInitialize()
        {
            // 半透明遮罩
            Image dim = UIFactory.CreateImage("Dim", transform, new Color(0f, 0f, 0f, 0.4f));
            UIFactory.Stretch(dim.rectTransform);

            // 面板
            Image panel = UIFactory.CreateImage("Panel", transform, new Color(0.14f, 0.14f, 0.17f, 1f));
            RectTransform panelRect = panel.rectTransform;
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(720f, 480f);
            panelRect.anchoredPosition = Vector2.zero;

            // 标题
            Text title = UIFactory.CreateText("Title", panel.transform, 26, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleCenter);
            title.text = PanelTitle;
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(24f, -52f);
            titleRect.offsetMax = new Vector2(-24f, -12f);

            // 占位提示
            Text hint = UIFactory.CreateText("Hint", panel.transform, 20, new Color(0.75f, 0.75f, 0.75f), TextAnchor.MiddleCenter);
            hint.text = "功能待实现";
            RectTransform hintRect = hint.rectTransform;
            hintRect.anchorMin = Vector2.zero;
            hintRect.anchorMax = Vector2.one;
            hintRect.offsetMin = new Vector2(40f, 80f);
            hintRect.offsetMax = new Vector2(-40f, -80f);

            // 关闭按钮
            Button close = UIFactory.CreateButton("Close", panel.transform, "关闭", 20);
            close.onClick.AddListener(Hide);
            RectTransform closeRect = close.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.5f, 0f);
            closeRect.anchorMax = new Vector2(0.5f, 0f);
            closeRect.pivot = new Vector2(0.5f, 0.5f);
            closeRect.anchoredPosition = new Vector2(0f, 40f);
            closeRect.sizeDelta = new Vector2(160f, 48f);
        }
    }
}
