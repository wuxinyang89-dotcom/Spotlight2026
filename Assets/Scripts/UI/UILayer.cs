namespace Spotlight.UI
{
    /// <summary>
    /// UI 层级：决定面板挂载到哪个层级节点、以及显示顺序。
    /// 数值越大层级越高（显示越靠前）。
    /// </summary>
    public enum UILayer
    {
        /// <summary>普通层：主界面、常驻 HUD、背包 / 任务等常规面板。</summary>
        Normal = 0,

        /// <summary>弹窗层：对话、确认框等覆盖在普通层之上的面板。</summary>
        Popup = 1,

        /// <summary>顶层：系统提示、加载遮罩等最高优先级的 UI。</summary>
        Top = 2
    }
}
