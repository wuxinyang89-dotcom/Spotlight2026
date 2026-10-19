using System.Collections.Generic;
using System.Linq;

namespace Spotlight.Data.Inventory
{
    /// <summary>
    /// 背包（运行时单例）：记录玩家已获取的道具。
    /// 道具定义为数据（<see cref="ItemDefinition"/>），背包只存引用，不写死内容。
    /// </summary>
    public sealed class Inventory
    {
        private static Inventory _instance;

        /// <summary>全局单例。</summary>
        public static Inventory Instance => _instance ?? (_instance = new Inventory());

        private readonly List<ItemDefinition> _owned = new List<ItemDefinition>();

        private Inventory()
        {
        }

        /// <summary>已拥有的道具（按获取顺序）。</summary>
        public IReadOnlyList<ItemDefinition> Owned => _owned;

        /// <summary>已拥有道具数量。</summary>
        public int Count => _owned.Count;

        /// <summary>获取道具（已拥有则忽略）。返回是否真的新增。</summary>
        public bool Add(ItemDefinition item)
        {
            if (item == null || _owned.Contains(item))
            {
                return false;
            }

            _owned.Add(item);
            return true;
        }

        /// <summary>是否拥有指定 id 的道具。</summary>
        public bool Has(string id) => _owned.Any(i => i.id == id);
    }
}
