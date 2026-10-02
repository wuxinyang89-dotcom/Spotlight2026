using UnityEngine;

namespace Spotlight.NPC
{
    /// <summary>
    /// NPC 移动行为入口（预留）。具体实现（巡逻、跟随、待机等）后续补充。
    /// 逻辑层只依赖本接口，不直接接触 GameObject。
    /// </summary>
    public interface INPCMovement
    {
        /// <summary>是否正在移动。</summary>
        bool IsMoving { get; }

        /// <summary>移动到目标点。</summary>
        void MoveTo(Vector3 destination);

        /// <summary>停止移动。</summary>
        void Stop();
    }
}
