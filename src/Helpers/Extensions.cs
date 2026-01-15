using System;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace HierarchicalStateMachine.Helpers
{
    public static class Extensions
    {
        public static IDictionary<K, V> AddOrUpdate<K, V>(this IDictionary<K, V> @this, K key, Func<K, V> add,
            Action<K, V> update)
        {
            if (@this.TryGetValue(key, out var thi))
                update(key, thi);
            else
                @this[key] = add(key);

            return @this;
        }
    }
}