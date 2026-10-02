using Spotlight.Character;

namespace Spotlight.NPC
{
    /// <summary>
    /// NPC 交互行为入口（预留）。具体实现（对话、交易、任务、跟随等）后续补充。
    /// 由 <see cref="NPC"/> 在被交互时转发调用。
    /// </summary>
    public interface INPCInteraction
    {
        /// <summary>当人物与 NPC 交互时触发。</summary>
        void OnInteract(ICharacter character);
    }
}
