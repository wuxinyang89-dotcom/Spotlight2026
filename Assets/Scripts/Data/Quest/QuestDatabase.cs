using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spotlight.Data.Quest
{
    /// <summary>
    /// 单个任务定义：id、标题、描述、当前目标文本与完成状态。内容均为数据。
    /// </summary>
    [Serializable]
    public sealed class QuestDefinition
    {
        public string id;
        public string title;

        [TextArea]
        public string description;

        /// <summary>当前目标（HUD 任务列表显示）。</summary>
        public string objective;

        public bool completed;
    }

    /// <summary>
    /// 任务数据库数据档：集中承载任务定义。由 QuestSystem 运行时读取。
    /// </summary>
    [CreateAssetMenu(fileName = "QuestDatabase", menuName = "Spotlight/Data/Quest Database", order = 2)]
    public sealed class QuestDatabase : ScriptableObject
    {
        [SerializeField] private List<QuestDefinition> quests = new List<QuestDefinition>();

        /// <summary>全部任务定义。</summary>
        public IReadOnlyList<QuestDefinition> Quests => quests;

        /// <summary>按 id 查找任务（未找到返回 null）。</summary>
        public QuestDefinition Find(string id)
        {
            foreach (QuestDefinition quest in quests)
            {
                if (quest != null && quest.id == id)
                {
                    return quest;
                }
            }

            return null;
        }

        /// <summary>追加一个任务定义（编辑器或代码创建数据档时使用）。</summary>
        public void AddQuest(QuestDefinition quest)
        {
            if (quest != null)
            {
                quests.Add(quest);
            }
        }
    }
}

