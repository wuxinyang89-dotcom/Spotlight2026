using Spotlight.UI;

namespace Spotlight.Gameplay
{
    /// <summary>
    /// 游戏内运行时错误弹窗（已统一到 UI 框架的弹窗系统）。
    /// 仅为保留原有调用入口的薄封装，实际显示由 Spotlight.UI.PopupDialog 完成。
    /// </summary>
    public static class RuntimeErrorPopup
    {
        /// <summary>显示错误弹窗（默认标题"运行错误"）。</summary>
        public static void ShowError(string message) => PopupDialog.ShowError(message);

        /// <summary>显示错误弹窗。</summary>
        public static void Show(string title, string message) => PopupDialog.Show(title, message);

        /// <summary>关闭错误弹窗。</summary>
        public static void Hide() => PopupDialog.Close();
    }
}
