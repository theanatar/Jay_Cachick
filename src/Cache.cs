using System;
using System.Collections.Generic;

namespace Jay_Cachick
{
    public class Cache <K, V, I> where I: CacheItem<K, V>
    {
        public int Size { get; private set; } //Size of the cache
        public event EventHandler<EvictedItemArgs<K,V>> ItemEvicted; //Notify Subscribers when an item gets evicetd. 

        private LinkedList<K> _queue; //Used to keep track of items and evict them if the cache is full
        private Dictionary<K, I> _cache; //Cache that keeps items in memory
        private object _lock = new object(); //Used for thread safe add and get


        public Cache(int size = 100)
        {
            Size = size;
            _queue = new LinkedList<K>();
            _cache = new Dictionary<K, I>();
        }

        public void Add(I item)
        {
            if (item == null) throw new ArgumentNullException("item","The Item cannot be null");
            if (item.Key == null) throw new ArgumentNullException("item.Key", "The key cannot be null");
            if (item.Value == null) throw new ArgumentNullException("item.Value", "The Value cannot be null");

            lock(_lock)
            {
                //If the item exists in the cache, update it
                if (_cache.ContainsKey(item.Key))
                {                    
                    _cache[item.Key].Value = item.Value;
                    _queue.Remove(_cache[item.Key].Node);
                    var node = _queue.AddFirst(item.Key);
                    _cache[item.Key].Node = node;
                }
                else
                {
                    var node = _queue.AddFirst(item.Key);
                    item.Node = node;
                    _cache.Add(item.Key, item);                    
                }

                EvictItem();
            }
        }

        public I Get(K key)
        {
            if (key == null) throw new ArgumentNullException();

            lock(_lock)
            {
                I item;
                if (_cache.TryGetValue(key, out item))
                {
                    _queue.Remove(item.Node);
                    var node = _queue.AddFirst(item.Key);
                    item.Node = node;
                    return item;
                }
            }

            throw new KeyNotFoundException();
        }

        public int Count()
        {
            return _cache.Count;
        }

        private void EvictItem()
        {
            if (_queue.Count <= Size) return;

            var lastNode = _queue.Last;
            if (lastNode == null) return;

            var item = _cache[lastNode.Value];
            _cache.Remove(lastNode.Value);
            _queue.RemoveLast();

            var args = new EvictedItemArgs<K, V>(item);
            ItemEvicted?.Invoke(this, args);
        }
    }
}
