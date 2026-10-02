using System.Collections.Generic;
using System.Linq;

namespace Spotlight.NPC.Attributes
{
    /// <summary>
    /// 一组 NPC 属性的组合，代表“某种 NPC”的属性构成。
    /// 内部按 <see cref="NPCAttributeType"/> 去重，每个类型最多保留一个取值，
    /// 通常由 <see cref="NPCAttributeCombinator"/> 排列组合生成。
    /// </summary>
    public sealed class NPCAttributeSet
    {
        private readonly Dictionary<NPCAttributeType, INPCAttribute> _attributes =
            new Dictionary<NPCAttributeType, INPCAttribute>();

        /// <summary>该组合包含的全部属性。</summary>
        public IReadOnlyCollection<INPCAttribute> Attributes => _attributes.Values;

        public NPCAttributeSet()
        {
        }

        public NPCAttributeSet(IEnumerable<INPCAttribute> attributes)
        {
            if (attributes == null)
            {
                return;
            }

            foreach (INPCAttribute attribute in attributes)
            {
                Add(attribute);
            }
        }

        /// <summary>添加（或覆盖同类型）属性。</summary>
        public void Add(INPCAttribute attribute)
        {
            if (attribute == null)
            {
                return;
            }

            _attributes[attribute.Type] = attribute;
        }

        /// <summary>是否包含指定类型的属性。</summary>
        public bool Has(NPCAttributeType type) => _attributes.ContainsKey(type);

        /// <summary>获取指定类型的属性（不存在返回 null）。</summary>
        public INPCAttribute Get(NPCAttributeType type)
        {
            return _attributes.TryGetValue(type, out INPCAttribute value) ? value : null;
        }

        /// <summary>获取指定类型并转换为具体属性类型（不存在或类型不符返回 null）。</summary>
        public T Get<T>(NPCAttributeType type) where T : class, INPCAttribute
        {
            return Get(type) as T;
        }

        /// <summary>尝试获取指定类型的属性。</summary>
        public bool TryGet(NPCAttributeType type, out INPCAttribute attribute)
        {
            return _attributes.TryGetValue(type, out attribute);
        }

        /// <summary>尝试获取指定类型并转换为具体属性类型。</summary>
        public bool TryGet<T>(NPCAttributeType type, out T attribute) where T : class, INPCAttribute
        {
            if (_attributes.TryGetValue(type, out INPCAttribute value) && value is T typed)
            {
                attribute = typed;
                return true;
            }

            attribute = null;
            return false;
        }

        public override string ToString()
        {
            return string.Join(", ", _attributes.Values.Select(a => a.ToString()));
        }
    }
}
