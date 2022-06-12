using System.Collections.Generic;

namespace Jay_Cachick
{
    public class CacheItem <K,V>
    {
        public K Key { get; private set; }
        public V Value { get; set; }
        internal LinkedListNode<K> Node { get; set; }

        public CacheItem(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
}
