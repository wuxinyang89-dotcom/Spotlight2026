using System;
using System.Collections.Generic;
using System.Linq;

namespace Spotlight.NPC.Attributes
{
    /// <summary>
    /// 属性排列组合器：把若干“并列属性”的候选集合做笛卡尔积，
    /// 生成所有可能的 <see cref="NPCAttributeSet"/>，从而组合出新 NPC。
    /// 例如：种族 × 性格 = 全部组合。
    /// </summary>
    public static class NPCAttributeCombinator
    {
        /// <summary>
        /// 对多个候选集合做笛卡尔积。
        /// 每个参数代表一个“并列属性维度”的全部候选值（如所有种族、所有性格）。
        /// </summary>
        public static IReadOnlyList<NPCAttributeSet> Combine(params IReadOnlyList<INPCAttribute>[] optionGroups)
        {
            if (optionGroups == null || optionGroups.Length == 0)
            {
                return Array.Empty<NPCAttributeSet>();
            }

            var results = new List<NPCAttributeSet>();
            CombineRecursive(optionGroups, 0, new List<INPCAttribute>(), results);
            return results;
        }

        /// <summary>等价于 <see cref="Combine(IReadOnlyList{INPCAttribute}[])"/> 的集合重载。</summary>
        public static IReadOnlyList<NPCAttributeSet> Combine(IEnumerable<IReadOnlyList<INPCAttribute>> optionGroups)
        {
            IReadOnlyList<INPCAttribute>[] groups =
                (optionGroups ?? Enumerable.Empty<IReadOnlyList<INPCAttribute>>()).ToArray();
            return Combine(groups);
        }

        private static void CombineRecursive(
            IReadOnlyList<IReadOnlyList<INPCAttribute>> groups,
            int groupIndex,
            List<INPCAttribute> current,
            List<NPCAttributeSet> results)
        {
            if (groupIndex >= groups.Count)
            {
                if (current.Count > 0)
                {
                    results.Add(new NPCAttributeSet(current));
                }

                return;
            }

            IReadOnlyList<INPCAttribute> group = groups[groupIndex];

            // 空分组：跳过该维度，继续组合后续分组。
            if (group == null || group.Count == 0)
            {
                CombineRecursive(groups, groupIndex + 1, current, results);
                return;
            }

            foreach (INPCAttribute attribute in group)
            {
                current.Add(attribute);
                CombineRecursive(groups, groupIndex + 1, current, results);
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}
