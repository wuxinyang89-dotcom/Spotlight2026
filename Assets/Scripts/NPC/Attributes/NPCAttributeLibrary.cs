using System.Collections.Generic;

namespace Spotlight.NPC.Attributes
{
    /// <summary>
    /// 属性候选库（预留）。此处仅放置少量占位数据用于演示与跑通组合逻辑；
    /// 实际的种族 / 性格定义、数量与内容后续可替换为配置表或数据驱动。
    /// </summary>
    public static class NPCAttributeLibrary
    {
        /// <summary>种族候选（占位）。</summary>
        public static readonly IReadOnlyList<RaceAttribute> Races = new List<RaceAttribute>
        {
            new RaceAttribute("Human", "人类"),
            new RaceAttribute("Elf", "精灵"),
            new RaceAttribute("Dwarf", "矮人"),
        };

        /// <summary>性格候选（占位）。</summary>
        public static readonly IReadOnlyList<PersonalityAttribute> Personalities = new List<PersonalityAttribute>
        {
            new PersonalityAttribute("Brave", "勇敢"),
            new PersonalityAttribute("Timid", "胆怯"),
            new PersonalityAttribute("Cheerful", "开朗"),
        };

        /// <summary>生成“种族 × 性格”的全部组合，用于快速产出不同 NPC。</summary>
        public static IReadOnlyList<NPCAttributeSet> CreateAllCombinations()
        {
            return NPCAttributeCombinator.Combine(Races, Personalities);
        }
    }
}
