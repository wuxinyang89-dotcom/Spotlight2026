using UnityEngine;

namespace Spotlight.Data.Dialogue
{
    /// <summary>
    /// 对话数据档（占位骨架）。
    /// 数据档字段尚未确定，请勿在此填入具体字段；待策划/需求明确后再补充。
    /// 该资产用于承载单个对话的内容，后续由触发交互的对象（NPC、可交互道具）引用。
    /// </summary>
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Spotlight/Data/Dialogue", order = 0)]
    public sealed class DialogueAsset : ScriptableObject
    {
        // TODO: 对话数据档字段待定，确定后在此补充。
    }
}
