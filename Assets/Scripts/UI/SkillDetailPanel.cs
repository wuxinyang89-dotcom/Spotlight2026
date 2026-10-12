namespace Spotlight.UI
{
    /// <summary>
    /// 技能详情面板（空壳）。具体界面内容后续实现。
    /// </summary>
    public sealed class SkillDetailPanel : PlaceholderPanel
    {
        protected override string PanelTitle => "技能详情";

        /// <summary>打开技能详情面板。</summary>
        public static void Open() => OpenPanel<SkillDetailPanel>();
    }
}
