using UnityEngine;
using Spotlight.Data.Inventory;
using Spotlight.Data.NPC;
using Spotlight.Data.Quest;

namespace Spotlight.Data
{
    /// <summary>
    /// 数据系统引导（常驻 DontDestroyOnLoad）：把各数据档 SO 注入 DataSystem，
    /// 使运行时可通过 DataSystem.Instance 统一访问。挂在初始场景任意物体上即可，全局唯一。
    /// 未拖入的数据档保持 null，不会注册。
    /// </summary>
    public sealed class DataBootstrap : MonoBehaviour
    {
        [Header("物品")]
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("任务")]
        [SerializeField] private QuestDatabase questDatabase;

        [Header("NPC 属性")]
        [SerializeField] private NPCAttributeLibraryAsset npcAttributeLibrary;

        private static DataBootstrap _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            if (itemDatabase != null)
            {
                DataSystem.Instance.Register(itemDatabase);
            }

            if (questDatabase != null)
            {
                DataSystem.Instance.Register(questDatabase);
            }

            if (npcAttributeLibrary != null)
            {
                DataSystem.Instance.Register(npcAttributeLibrary);
            }
        }
    }
}
