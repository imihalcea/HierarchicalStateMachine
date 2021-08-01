using System;
using System.Collections.Generic;

namespace HierarchicalStateMachine.Helpers
{
    public static class Extensions
    {
        public static IDictionary<K, V> AddOrUpdate<K, V>(this IDictionary<K, V> @this, K key, Func<K, V> add,
            Func<K, V, V> update)
        {
            if (@this.ContainsKey(key))
                @this[key] = update(key, @this[key]);
            else
                @this[key] = add(key);

            return @this;
        }
        
        public static IDictionary<K, V> AddOrUpdate<K, V>(this IDictionary<K, V> @this, K key, Func<K, V> add,
            Action<K, V> update)
        {
            if (@this.ContainsKey(key))
                update(key, @this[key]);
            else
                @this[key] = add(key);

            return @this;
        }
    }
}