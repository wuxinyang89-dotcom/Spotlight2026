namespace Spotlight.NPC.Attributes
{
    /// <summary>
    /// NPC 属性类型。当前仅包含种族与性格两类；
    /// 新增类型时在此追加枚举值，并实现对应的 <see cref="INPCAttribute"/>。
    /// </summary>
    public enum NPCAttributeType
    {
        /// <summary>种族。</summary>
        Race,

        /// <summary>性格。</summary>
        Personality
    }
}
