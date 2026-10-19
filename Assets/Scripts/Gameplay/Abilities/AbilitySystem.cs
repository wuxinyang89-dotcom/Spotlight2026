using System.Collections.Generic;
using UnityEngine;

namespace Spotlight.Gameplay.Abilities
{
    /// <summary>
    /// 能力系统（运行时单例）：持有技能库（数据档）与玩家当前拥有的能力，
    /// 提供“对目标施加能力”的行为实现（行为在代码，数值来自数据档）。
    /// 无数据档注入时使用内置默认，保证可运行。
    /// </summary>
    public sealed class AbilitySystem
    {
        private static AbilitySystem _instance;

        /// <summary>全局单例。</summary>
        public static AbilitySystem Instance => _instance ?? (_instance = new AbilitySystem());

        private readonly List<AbilityType> _owned = new List<AbilityType>();

        private AbilitySystem()
        {
        }

        /// <summary>技能库数据档（未注入时使用内置默认）。</summary>
        public SkillDatabase Database { get; private set; }

        /// <summary>玩家当前拥有的能力。</summary>
        public IReadOnlyList<AbilityType> Owned => _owned;

        /// <summary>注入技能库（传 null 则使用内置默认，保证可运行）。</summary>
        public void Register(SkillDatabase database)
        {
            Database = database != null ? database : SkillDatabase.CreateDefault();
        }

        /// <summary>授予能力（去重）。</summary>
        public void Grant(AbilityType type)
        {
            if (!_owned.Contains(type))
            {
                _owned.Add(type);
            }
        }

        /// <summary>清空已拥有能力。</summary>
        public void ClearAbilities() => _owned.Clear();

        /// <summary>是否拥有指定能力。</summary>
        public bool Has(AbilityType type) => _owned.Contains(type);

        /// <summary>获取能力定义（未注入技能库或未找到时返回 null）。</summary>
        public AbilityDefinition GetDefinition(AbilityType type) => Database != null ? Database.Find(type) : null;

        /// <summary>
        /// 对目标刚体施加能力效果（目标为受能力影响的物理对象，见 <see cref="IAbilityTarget"/>）。
        /// 具体行为按 <paramref name="type"/> 解释，数值取自对应能力定义。
        /// </summary>
        public void Apply(AbilityType type, IAbilityTarget target, Vector3 fromPosition)
        {
            if (target == null || target.Body == null)
            {
                return;
            }

            AbilityDefinition def = GetDefinition(type);
            float massMul = def != null ? def.massMultiplier : 1f;
            float impulse = def != null ? def.impulseForce : 5f;
            float lift = def != null ? def.liftForce : 0f;

            switch (type)
            {
                case AbilityType.Levity:
                    // 减轻质量 + 略微抬升（悬浮）。
                    target.Body.mass = Mathf.Max(0.1f, target.Body.mass * massMul);
                    if (lift > 0f)
                    {
                        target.Body.AddForce(new Vector2(0f, lift), ForceMode2D.Impulse);
                    }
                    break;

                case AbilityType.Melancholy:
                    // 增加重量 + 标记脆弱。
                    target.Body.mass *= massMul;
                    if (def == null || def.makeFragile)
                    {
                        target.MarkFragile();
                    }
                    break;

                case AbilityType.Fervor:
                    // 朝远离玩家的方向施加大冲量；对脆弱对象直接打碎。
                    ApplyDirectionalImpulse(target.Body, fromPosition, impulse);
                    if (target.Fragile && (def == null || def.breakFragile))
                    {
                        target.Break();
                    }
                    break;

                case AbilityType.Isolation:
                    // 对附近物体产生推力（远离玩家）。
                    ApplyDirectionalImpulse(target.Body, fromPosition, impulse);
                    break;
            }
        }

        private static void ApplyDirectionalImpulse(Rigidbody2D body, Vector3 fromPosition, float impulse)
        {
            Vector2 dir = ((Vector2)body.transform.position - (Vector2)fromPosition);
            if (dir.sqrMagnitude < 0.0001f)
            {
                dir = Vector2.right;
            }
            else
            {
                dir.Normalize();
            }

            body.AddForce(dir * impulse, ForceMode2D.Impulse);
        }
    }
}
