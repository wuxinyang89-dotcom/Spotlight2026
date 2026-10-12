using System;
using System.Collections.Generic;

namespace Spotlight.UI
{
    /// <summary>
    /// UI 面板管理器（全局单例，纯逻辑）：负责面板注册、按类型开关，以及"返回栈"管理。
    /// 面板对象由 UIPanel 在 Awake 时自动注册；打开时会把面板挂载到 UIRoot 的对应层级。
    /// </summary>
    public sealed class UIManager
    {
        private static UIManager _instance;

        private readonly Dictionary<Type, UIPanel> _panels = new Dictionary<Type, UIPanel>();
        private readonly List<UIPanel> _openStack = new List<UIPanel>();

        /// <summary>全局单例。</summary>
        public static UIManager Instance => _instance ?? (_instance = new UIManager());

        private UIManager() { }

        /// <summary>注册面板（同类型重复注册会覆盖）。</summary>
        public void Register(UIPanel panel)
        {
            if (panel == null)
            {
                return;
            }

            _panels[panel.GetType()] = panel;
        }

        /// <summary>注销面板。</summary>
        public void Unregister(UIPanel panel)
        {
            if (panel == null)
            {
                return;
            }

            _panels.Remove(panel.GetType());
            _openStack.RemoveAll(p => p == panel);
        }

        /// <summary>获取已注册面板（未注册返回 null）。</summary>
        public T Get<T>() where T : UIPanel
        {
            return _panels.TryGetValue(typeof(T), out UIPanel panel) ? panel as T : null;
        }

        /// <summary>打开面板（挂载到对应层级并显示，压入返回栈）。</summary>
        public T Open<T>() where T : UIPanel
        {
            T panel = Get<T>();
            if (panel == null)
            {
                return null;
            }

            UIRoot.Ensure().Attach(panel);
            panel.Show();

            _openStack.RemoveAll(p => p == panel);
            _openStack.Add(panel);
            return panel;
        }

        /// <summary>关闭面板（隐藏并从返回栈移除）。</summary>
        public void Close<T>() where T : UIPanel
        {
            T panel = Get<T>();
            if (panel == null)
            {
                return;
            }

            panel.Hide();
            _openStack.RemoveAll(p => p == panel);
        }

        /// <summary>关闭返回栈顶面板（"返回"），返回是否真的关闭了面板。</summary>
        public bool Back()
        {
            if (_openStack.Count == 0)
            {
                return false;
            }

            UIPanel top = _openStack[_openStack.Count - 1];
            _openStack.RemoveAt(_openStack.Count - 1);
            top?.Hide();
            return true;
        }

        /// <summary>关闭所有已打开的面板。</summary>
        public void CloseAll()
        {
            foreach (UIPanel panel in _openStack)
            {
                panel?.Hide();
            }

            _openStack.Clear();
        }

        /// <summary>当前是否有已打开的面板。</summary>
        public bool HasOpenPanels => _openStack.Count > 0;
    }
}
