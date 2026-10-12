using UnityEngine;

namespace Spotlight.UI
{
    /// <summary>
    /// UI 面板基类：挂载在 UI 根（UIRoot）层级节点下的面板 MonoBehaviour。
    /// 面板 GameObject 在场景中保持激活，基类在 Awake 时自动注册到 UIManager，
    /// 并根据 showOnStart 决定初始是否显示（一般面板默认隐藏，等待打开）。
    /// 子类通过重写 OnInitialize / OnShow / OnHide 绑定控件与刷新数据。
    /// 面板会作为全屏容器：首次显示前自动确保 RectTransform 存在并铺满父节点，
    /// 从而使子元素的锚点定位有正确的参考。
    /// 注意：若子类重写 Awake，务必先调用 base.Awake() 以保证自注册。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIPanel : MonoBehaviour
    {
        [SerializeField, Tooltip("场景加载后是否立即显示（一般面板为 false）。")]
        private bool showOnStart = false;

        private bool _initialized;

        /// <summary>面板所属层级（子类可重写以调整挂载层级与显示顺序）。</summary>
        public virtual UILayer Layer => UILayer.Normal;

        /// <summary>当前是否可见。</summary>
        public bool IsVisible { get; private set; }

        protected virtual void Awake()
        {
            UIManager.Instance.Register(this);

            if (showOnStart)
            {
                Show();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>显示面板（首次显示前会调用一次 OnInitialize）。</summary>
        public void Show()
        {
            if (!_initialized)
            {
                _initialized = true;
                EnsureFullRect();
                OnInitialize();
            }

            if (IsVisible)
            {
                return;
            }

            IsVisible = true;
            gameObject.SetActive(true);
            OnShow();
        }

        /// <summary>隐藏面板。</summary>
        public void Hide()
        {
            if (!IsVisible)
            {
                return;
            }

            IsVisible = false;
            OnHide();
            gameObject.SetActive(false);
        }

        /// <summary>首次显示前调用一次：用于缓存子控件引用、初始化数据结构等。</summary>
        protected virtual void OnInitialize() { }

        /// <summary>每次显示时调用：用于把数据刷新到控件。</summary>
        protected virtual void OnShow() { }

        /// <summary>每次隐藏时调用：用于清理或取消订阅。</summary>
        protected virtual void OnHide() { }

        /// <summary>确保面板容器 RectTransform 存在并铺满父节点（面板作为全屏容器）。</summary>
        private void EnsureFullRect()
        {
            RectTransform rect = GetComponent<RectTransform>();
            if (rect == null)
            {
                rect = gameObject.AddComponent<RectTransform>();
            }

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
