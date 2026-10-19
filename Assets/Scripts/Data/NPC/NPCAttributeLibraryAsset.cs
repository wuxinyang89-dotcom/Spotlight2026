using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Spotlight.NPC.Appearance;
using Spotlight.NPC.Attributes;

namespace Spotlight.Data.NPC
{
    /// <summary>
    /// NPC 属性库数据档：承载种族 / 性格的取值定义。
    /// 当前字段与 INPCAttribute 已确定的契约（Id / 显示名）一致，效果、数值等字段待定后补充。
    /// 提供把条目转换为运行时 RaceAttribute / PersonalityAttribute 的方法，并可产出
    /// 全部 "种族 × 性格" 组合（NPCAttributeSet），供 NPC 等现有逻辑使用。
    /// </summary>
    [CreateAssetMenu(fileName = "NPCAttributeLibrary", menuName = "Spotlight/Data/NPC Attribute Library", order = 3)]
    public sealed class NPCAttributeLibraryAsset : ScriptableObject
    {
        [Header("种族")]
        [SerializeField] private List<NPCAttributeEntry> races = new List<NPCAttributeEntry>();

        [Header("性格")]
        [SerializeField] private List<NPCAttributeEntry> personalities = new List<NPCAttributeEntry>();

        /// <summary>种族取值（转换为运行时属性，跳过空 Id 的条目）。</summary>
        public IReadOnlyList<RaceAttribute> GetRaces() =>
            races.Where(e => !string.IsNullOrEmpty(e.id))
                 .Select(e => new RaceAttribute(e.id, e.displayName, e.appearance))
                 .ToList();

        /// <summary>性格取值（转换为运行时属性，跳过空 Id 的条目）。</summary>
        public IReadOnlyList<PersonalityAttribute> GetPersonalities() =>
            personalities.Where(e => !string.IsNullOrEmpty(e.id))
                         .Select(e => new PersonalityAttribute(e.id, e.displayName))
                         .ToList();

        /// <summary>生成 "种族 × 性格" 的全部组合，用于快速产出不同 NPC。</summary>
        public IReadOnlyList<NPCAttributeSet> CreateAllCombinations() =>
            NPCAttributeCombinator.Combine(GetRaces(), GetPersonalities());
    }

    /// <summary>
    /// 属性取值条目：一条记录对应一个种族或性格取值。
    /// 字段与 INPCAttribute 已确定的契约（Id / 显示名）一致；其余字段待定后在此补充。
    /// </summary>
    [Serializable]
    public sealed class NPCAttributeEntry
    {
        /// <summary>取值唯一标识（如 "Human"、"Brave"）。</summary>
        public string id;

        /// <summary>显示名称（如 "人类"、"勇敢"）。</summary>
        public string displayName;

        /// <summary>种族外观（仅种族使用；性格留空）。</summary>
        public NPCAppearanceAsset appearance;
    }
}

