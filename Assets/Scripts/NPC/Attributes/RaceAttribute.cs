using Spotlight.NPC.Appearance;

namespace Spotlight.NPC.Attributes
{
    /// <summary>
    /// 种族属性。种族会外化在外貌上，故持有 <see cref="Appearance"/>（身体部件外观数据档）。
    /// 种族特有的数值、技能等后续补充。
    /// </summary>
    public sealed class RaceAttribute : INPCAttribute
    {
        public NPCAttributeType Type => NPCAttributeType.Race;

        public string Id { get; }

        public string DisplayName { get; }

        /// <summary>种族外观（外化在外貌上；未设置时为 null）。</summary>
        public NPCAppearanceAsset Appearance { get; }

        public RaceAttribute(string id, string displayName = null, NPCAppearanceAsset appearance = null)
        {
            Id = id;
            DisplayName = string.IsNullOrEmpty(displayName) ? id : displayName;
            Appearance = appearance;
        }

        public override string ToString() => $"Race({Id})";
    }
}

