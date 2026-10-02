namespace Spotlight.NPC.Attributes
{
    /// <summary>
    /// 种族属性（预留实现）。当前仅承载标识信息；
    /// 种族特有的数值、技能、外观等后续补充。
    /// </summary>
    public sealed class RaceAttribute : INPCAttribute
    {
        public NPCAttributeType Type => NPCAttributeType.Race;

        public string Id { get; }

        public string DisplayName { get; }

        public RaceAttribute(string id, string displayName = null)
        {
            Id = id;
            DisplayName = string.IsNullOrEmpty(displayName) ? id : displayName;
        }

        public override string ToString() => $"Race({Id})";
    }
}
