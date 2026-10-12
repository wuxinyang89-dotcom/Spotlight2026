namespace Spotlight.UI
{
    /// <summary>
    /// 背包详情面板（空壳）。具体界面内容后续实现。
    /// </summary>
    public sealed class InventoryPanel : PlaceholderPanel
    {
        protected override string PanelTitle => "背包";

        /// <summary>打开背包详情面板。</summary>
        public static void Open() => OpenPanel<InventoryPanel>();
    }
}
