using System.Collections.Generic;
using UnityEngine;

namespace Spotlight.Gameplay.Abilities
{
    /// <summary>
    /// 技能库数据档：承载一组能力定义。
    /// 由 <see cref="AbilitySystem"/> 注入后，运行时据此查询能力的表现与数值参数。
    /// </summary>
    [CreateAssetMenu(fileName = "SkillDatabase", menuName = "Spotlight/Data/Skill Database", order = 4)]
    public sealed class SkillDatabase : ScriptableObject
    {
        [SerializeField] private List<AbilityDefinition> abilities = new List<AbilityDefinition>();

        /// <summary>全部能力定义。</summary>
        public IReadOnlyList<AbilityDefinition> Abilities => abilities;

        /// <summary>按类型查找能力定义（未找到返回 null）。</summary>
        public AbilityDefinition Find(AbilityType type)
        {
            foreach (AbilityDefinition ability in abilities)
            {
                if (ability != null && ability.type == type)
                {
                    return ability;
                }
            }

            return null;
        }

        /// <summary>
        /// 创建内置默认技能库（无数据档时的占位回退，保证可运行）。
        /// 正式内容应以真实 SkillDatabase 数据档覆盖。
        /// </summary>
        public static SkillDatabase CreateDefault()
        {
            SkillDatabase db = ScriptableObject.CreateInstance<SkillDatabase>();

            db.abilities.Add(CreateDefault(
                AbilityType.Levity, "Levity",
                "减轻物体质量 / 自身悬浮，越过深坑。",
                new Color(0.4f, 0.85f, 1f),
                massMul: 0.3f, impulse: 0f, lift: 3f, fragile: false, breakFragile: false,
                levitateDuration: 3f, selfImpulse: 0f));

            db.abilities.Add(CreateDefault(
                AbilityType.Melancholy, "Melancholy",
                "增加物体重量，使其变得更脆弱易碎。",
                new Color(0.45f, 0.4f, 0.9f),
                massMul: 2.5f, impulse: 0f, lift: 0f, fragile: true, breakFragile: false,
                levitateDuration: 0f, selfImpulse: 0f));

            db.abilities.Add(CreateDefault(
                AbilityType.Fervor, "Fervor",
                "物体获得更大加速度 / 撞碎脆弱物体。",
                new Color(1f, 0.5f, 0.3f),
                massMul: 1f, impulse: 8f, lift: 0f, fragile: false, breakFragile: true,
                levitateDuration: 0f, selfImpulse: 0f));

            db.abilities.Add(CreateDefault(
                AbilityType.Isolation, "Isolation",
                "对附近物体产生推力，并借反作用力获得动能。",
                new Color(0.4f, 1f, 0.6f),
                massMul: 1f, impulse: 7f, lift: 0f, fragile: false, breakFragile: false,
                levitateDuration: 0f, selfImpulse: 8f));

            return db;
        }

        private static AbilityDefinition CreateDefault(
            AbilityType type, string name, string desc, Color color,
            float massMul, float impulse, float lift, bool fragile, bool breakFragile,
            float levitateDuration, float selfImpulse)
        {
            AbilityDefinition def = ScriptableObject.CreateInstance<AbilityDefinition>();
            def.type = type;
            def.displayName = name;
            def.description = desc;
            def.orbColor = color;
            def.massMultiplier = massMul;
            def.impulseForce = impulse;
            def.liftForce = lift;
            def.makeFragile = fragile;
            def.breakFragile = breakFragile;
            def.levitateDuration = levitateDuration;
            def.selfImpulseForce = selfImpulse;
            return def;
        }
    }
}
