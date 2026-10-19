using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spotlight.Data.Inventory
{
    /// <summary>
    /// 单个道具定义：id、名称、简介与图标（美术插槽）。
    /// 内容均为数据，不写死在代码中；图标留空时回退程序化占位色块。
    /// </summary>
    [Serializable]
    public sealed class ItemDefinition
    {
        /// <summary>唯一标识（如 "Translator"）。</summary>
        public string id;

        /// <summary>名称。</summary>
        public string displayName;

        /// <summary>简介。</summary>
        [TextArea]
        public string description;

        /// <summary>图标（美术插槽；留空则用 iconColor 占位色块）。</summary>
        public Sprite icon;

        /// <summary>占位颜色（无图标时用于程序化占位）。</summary>
        public Color iconColor = Color.white;
    }

    /// <summary>
    /// 物品数据库数据档：集中承载道具定义。
    /// 由 DataBootstrap 注入 DataSystem，运行时按 id 查询。
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Spotlight/Data/Item Database", order = 1)]
    public sealed class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemDefinition> items = new List<ItemDefinition>();

        /// <summary>全部道具定义。</summary>
        public IReadOnlyList<ItemDefinition> Items => items;

        /// <summary>按 id 查找道具（未找到返回 null）。</summary>
        public ItemDefinition Find(string id)
        {
            foreach (ItemDefinition item in items)
            {
                if (item != null && item.id == id)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>追加一个道具定义（编辑器或代码创建数据档时使用）。</summary>
        public void AddItem(ItemDefinition item)
        {
            if (item != null)
            {
                items.Add(item);
            }
        }
    }
}

