using Spotlight.Data.Inventory;
using Spotlight.Data.NPC;
using Spotlight.Data.Quest;

namespace Spotlight.Data
{
    /// <summary>
    /// 统一数据访问层（全局单例）：集中管理各"数据库型"数据档 SO 的引用，供运行时按需获取。
    /// 对话（DialogueAsset）为单资产型，由交互物直接引用，不在此注册。
    /// 由 DataBootstrap 在场景启动时把各数据档 SO 注入（Register）。
    /// </summary>
    public sealed class DataSystem
    {
        private static DataSystem _instance;

        /// <summary>全局单例。</summary>
        public static DataSystem Instance => _instance ?? (_instance = new DataSystem());

        /// <summary>物品数据库（未注册时为 null）。</summary>
        public ItemDatabase Items { get; private set; }

        /// <summary>任务数据库（未注册时为 null）。</summary>
        public QuestDatabase Quests { get; private set; }

        /// <summary>NPC 属性库（未注册时为 null）。</summary>
        public NPCAttributeLibraryAsset NPCAttributes { get; private set; }

        private DataSystem()
        {
        }

        /// <summary>注册物品数据库。</summary>
        public void Register(ItemDatabase database) => Items = database;

        /// <summary>注册任务数据库。</summary>
        public void Register(QuestDatabase database) => Quests = database;

        /// <summary>注册 NPC 属性库。</summary>
        public void Register(NPCAttributeLibraryAsset database) => NPCAttributes = database;
    }
}
