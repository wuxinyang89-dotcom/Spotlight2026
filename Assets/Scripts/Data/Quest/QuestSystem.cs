using System.Collections.Generic;

namespace Spotlight.Data.Quest
{
    /// <summary>
    /// 任务系统（运行时单例）：管理当前激活的任务与目标。
    /// 任务内容为数据（QuestDefinition / QuestDatabase），不写死在代码中。
    /// </summary>
    public sealed class QuestSystem
    {
        private static QuestSystem _instance;

        /// <summary>全局单例。</summary>
        public static QuestSystem Instance => _instance ?? (_instance = new QuestSystem());

        private readonly List<QuestDefinition> _active = new List<QuestDefinition>();

        private QuestSystem()
        {
        }

        /// <summary>当前激活的任务（按顺序）。</summary>
        public IReadOnlyList<QuestDefinition> Active => _active;

        /// <summary>当前主任务（无任务为 null）。</summary>
        public QuestDefinition Current => _active.Count > 0 ? _active[0] : null;

        /// <summary>当前目标文本（供 HUD 显示；无任务返回 null）。</summary>
        public string CurrentObjective => Current != null ? Current.objective : null;

        /// <summary>注入任务数据库并加载未完成的任务。</summary>
        public void Register(QuestDatabase database)
        {
            if (database == null)
            {
                return;
            }

            _active.Clear();
            foreach (QuestDefinition quest in database.Quests)
            {
                if (quest != null && !quest.completed)
                {
                    _active.Add(quest);
                }
            }
        }

        /// <summary>开始一个任务（已激活则忽略）。</summary>
        public void StartQuest(QuestDefinition quest)
        {
            if (quest != null && !_active.Contains(quest))
            {
                _active.Add(quest);
            }
        }

        /// <summary>完成指定 id 的任务。</summary>
        public void CompleteQuest(string id)
        {
            _active.RemoveAll(q => q != null && q.id == id);
        }
    }
}
