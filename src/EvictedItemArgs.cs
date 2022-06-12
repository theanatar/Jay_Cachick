using System;

namespace Jay_Cachick
{
    public class EvictedItemArgs<K, V> : EventArgs
    {
        public CacheItem<K, V> Item { get; set; }

        public EvictedItemArgs(CacheItem<K, V> item)
        {
            Item = item;
        }
    }
}
