namespace Spotlight.NPC.Appearance
{
    /// <summary>
    /// NPC 身体部件类型。枚举顺序即渲染层级（从底到顶）：
    /// 身体最底，前发最顶。肤色作为颜色外化在身体部件上。
    /// </summary>
    public enum NPCBodyPartType
    {
        /// <summary>身体（仅 1 个部件，肤色着色）。</summary>
        Body = 0,

        /// <summary>后发（底层头发）。</summary>
        BackHair = 1,

        /// <summary>侧发 / 辫子。</summary>
        SideHair = 2,

        /// <summary>眼睛。</summary>
        Eyes = 3,

        /// <summary>鼻子。</summary>
        Nose = 4,

        /// <summary>嘴巴。</summary>
        Mouth = 5,

        /// <summary>前发（顶层头发）。</summary>
        FrontHair = 6
    }
}
