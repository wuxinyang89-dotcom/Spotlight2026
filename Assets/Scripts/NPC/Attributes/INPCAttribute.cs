namespace Spotlight.NPC.Attributes
{
    /// <summary>
    /// NPC 单项属性的抽象。一个属性对应一个 <see cref="NPCAttributeType"/>，
    /// 并以 Id 唯一标识具体取值（如 “Human”“Brave”）。
    /// 具体属性内容（效果、数值、描述等）为预留项。
    /// </summary>
    public interface INPCAttribute
    {
        /// <summary>属性类型。</summary>
        NPCAttributeType Type { get; }

        /// <summary>属性取值的唯一标识。</summary>
        string Id { get; }

        /// <summary>显示名称。</summary>
        string DisplayName { get; }
    }
}
