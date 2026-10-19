using System;
using UnityEngine;

namespace Spotlight.NPC.Appearance
{
    /// <summary>单个身体部件的定义：Sprite（美术插槽）+ 着色。</summary>
    [Serializable]
    public sealed class NPCPartDefinition
    {
        [Tooltip("部件贴图（留空则不渲染该部件）")]
        public Sprite sprite;

        [Tooltip("部件着色（头发 / 眼睛 / 五官颜色等）")]
        public Color color = Color.white;
    }

    /// <summary>
    /// NPC 外观数据档：把 NPC 按身体部件分解（纸娃娃）。
    /// 身体仅 1 个部件（肤色外化在其上）；头部细分为前发 / 后发 / 侧发 / 眼睛 / 鼻子 / 嘴巴。
    /// 所有 Sprite 均为美术插槽，留空则回退程序化占位（保证无美术也能跑通）。
    /// 种族属性通过 <see cref="Spotlight.NPC.Attributes.RaceAttribute"/> 引用本数据档，使种族外化在外貌上。
    /// </summary>
    [CreateAssetMenu(fileName = "NPCAppearance", menuName = "Spotlight/NPC/NPC Appearance", order = 20)]
    public sealed class NPCAppearanceAsset : ScriptableObject
    {
        [Header("肤色（外化在身体部件上）")]
        public Color skinColor = new Color(1f, 0.9f, 0.8f);

        [Header("身体（仅 1 个部件）")]
        public NPCPartDefinition body = new NPCPartDefinition();

        [Header("头部（细分部件）")]
        public NPCPartDefinition backHair = new NPCPartDefinition();
        public NPCPartDefinition sideHair = new NPCPartDefinition();
        public NPCPartDefinition eyes = new NPCPartDefinition();
        public NPCPartDefinition nose = new NPCPartDefinition();
        public NPCPartDefinition mouth = new NPCPartDefinition();
        public NPCPartDefinition frontHair = new NPCPartDefinition();

        /// <summary>按部件类型取部件定义（未知类型返回 null）。</summary>
        public NPCPartDefinition GetPart(NPCBodyPartType type)
        {
            switch (type)
            {
                case NPCBodyPartType.Body: return body;
                case NPCBodyPartType.BackHair: return backHair;
                case NPCBodyPartType.SideHair: return sideHair;
                case NPCBodyPartType.Eyes: return eyes;
                case NPCBodyPartType.Nose: return nose;
                case NPCBodyPartType.Mouth: return mouth;
                case NPCBodyPartType.FrontHair: return frontHair;
                default: return null;
            }
        }

        /// <summary>渲染顺序（从底到顶）。</summary>
        public static readonly NPCBodyPartType[] RenderOrder =
        {
            NPCBodyPartType.Body,
            NPCBodyPartType.BackHair,
            NPCBodyPartType.SideHair,
            NPCBodyPartType.Eyes,
            NPCBodyPartType.Nose,
            NPCBodyPartType.Mouth,
            NPCBodyPartType.FrontHair
        };
    }
}
