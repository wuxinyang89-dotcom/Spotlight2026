using UnityEngine;

namespace Spotlight.Gameplay.Abilities
{
    /// <summary>
    /// 单个能力的数据定义（数据档）。
    /// 名称、简介、图标（美术插槽）、光球颜色与数值参数均为“数据”，不在代码中写死内容。
    /// 行为（每种能力做什么）由 <see cref="AbilitySystem"/> 依据 <see cref="type"/> 解释，
    /// 数值与表现则从本数据档读取，实现“内容数据化、行为代码化”。
    /// </summary>
    [CreateAssetMenu(fileName = "AbilityDefinition", menuName = "Spotlight/Data/Ability Definition", order = 3)]
    public sealed class AbilityDefinition : ScriptableObject
    {
        [Header("标识")]
        public AbilityType type;

        [Header("表现")]
        public string displayName = "能力";

        [TextArea]
        public string description = string.Empty;

        [Header("美术插槽（留空则回退程序化占位）")]
        public Sprite icon;                 // 技能图标
        public Color orbColor = Color.white; // 技能环光球颜色

        [Header("数值参数（按能力类型取用）")]
        [Min(0f)] public float massMultiplier = 1f; // Levity / Melancholy：质量倍率
        [Min(0f)] public float impulseForce = 5f;   // Fervor / Isolation：冲量
        [Min(0f)] public float liftForce = 3f;      // Levity：悬浮抬升力
        public bool makeFragile;                    // Melancholy：使对象变脆弱
        public bool breakFragile;                   // Fervor：可打碎脆弱对象
        [Min(0f)] public float levitateDuration = 3f; // Levity：自身悬浮时长（秒）
        [Min(0f)] public float selfImpulseForce = 8f; // Isolation：借力反冲（施放者获得冲量）
    }
}
