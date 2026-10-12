namespace Spotlight.UI
{
    /// <summary>
    /// 任务日志面板（空壳）。具体界面内容后续实现。
    /// </summary>
    public sealed class TaskLogPanel : PlaceholderPanel
    {
        protected override string PanelTitle => "任务日志";

        /// <summary>打开任务日志面板。</summary>
        public static void Open() => OpenPanel<TaskLogPanel>();
    }
}
