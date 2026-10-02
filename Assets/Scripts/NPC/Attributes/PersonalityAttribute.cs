namespace Spotlight.NPC.Attributes
{
    /// <summary>
    /// 性格属性（预留实现）。当前仅承载标识信息；
    /// 性格对行为、对话风格等的影响后续补充。
    /// </summary>
    public sealed class PersonalityAttribute : INPCAttribute
    {
        public NPCAttributeType Type => NPCAttributeType.Personality;

        public string Id { get; }

        public string DisplayName { get; }

        public PersonalityAttribute(string id, string displayName = null)
        {
            Id = id;
            DisplayName = string.IsNullOrEmpty(displayName) ? id : displayName;
        }

        public override string ToString() => $"Personality({Id})";
    }
}
