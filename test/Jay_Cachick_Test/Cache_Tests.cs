using Jay_Cachick;
using Jay_Cachick_Test.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Jay_Cachick_Test
{
    public class Tests
    {
        [Test]
        public void AddSimple()
        {
            var cache = new Cache<int, string, MainCacheItem>();
            cache.Add(new MainCacheItem(1, "Barry Allen"));
            Assert.AreEqual(cache.Get(1).Value, "Barry Allen");
        }

        [Test]
        public void AddMultiple()
        {
            var cache = new Cache<int, string, MainCacheItem>(3);
            cache.Add(new MainCacheItem(1, "Barry Allen"));
            cache.Add(new MainCacheItem(2, "Wally West"));
            cache.Add(new MainCacheItem(3, "Bart Allen"));
            Assert.AreEqual(cache.Get(1).Value, "Barry Allen");
            Assert.AreEqual(cache.Get(2).Value, "Wally West");
            Assert.AreEqual(cache.Get(3).Value, "Bart Allen");
        }

        [Test]
        public void AddWithEviction()
        {
            var cache = new Cache<int, string, MainCacheItem>(2);
            cache.Add(new MainCacheItem(1, "Barry Allen"));
            cache.Add(new MainCacheItem(2, "Wally West"));
            cache.Add(new MainCacheItem(3, "Bart Allen"));

            Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
            Assert.AreEqual(cache.Get(2).Value, "Wally West");
            Assert.AreEqual(cache.Get(3).Value, "Bart Allen");
        }

        [Test]
        public void UpdateValue()
        {
            var cache = new Cache<int, string, MainCacheItem>();
            cache.Add(new MainCacheItem(1, "Barry Allen"));
            cache.Add(new MainCacheItem(2, "Wally West"));
            Assert.AreEqual(cache.Get(1).Value, "Barry Allen");

            cache.Add(new MainCacheItem(1, "Bart Allen"));
            Assert.AreEqual(cache.Get(1).Value, "Bart Allen");
            Assert.AreEqual(cache.Get(2).Value, "Wally West");
        }

        [Test]
        public void AddMany()
        {
            var cacheSize = 50;
            var cache = new Cache<int, string, MainCacheItem>(cacheSize);

            var itemsToAdd = 100;
            for (var i = 0; i < itemsToAdd; i++)
            {
                cache.Add(new MainCacheItem(i, i.ToString()));
            }

            for (var i = 99; i > cacheSize; i--)
            {
                Assert.AreEqual(cache.Get(i).Value, i.ToString());
            }
        }

        [Test]
        public void AddManyParallel()
        {
            var cacheSize = 50;
            var cache = new Cache<int, string, MainCacheItem>(cacheSize);
            
            var itemsToAdd = 100;
            var threads = Process.GetCurrentProcess().Threads;

            Parallel.For(0, threads.Count, a =>
            {
                for (var i = 0; i < itemsToAdd; i++)
                {
                    cache.Add(new MainCacheItem(i, i.ToString()));
                }
            });

            Assert.AreEqual(cache.Count(), cacheSize);
        }

        [Test]
        public void AddWithGetEviction()
        {
            var cacheSize = 3;
            var cache = new Cache<int, string, MainCacheItem>(cacheSize);

            cache.Add(new MainCacheItem(1, "Barry Allen"));
            cache.Add(new MainCacheItem(2, "Wally West"));
            cache.Add(new MainCacheItem(3, "Bart Allen"));

            //Get first item to be evicted
            Assert.AreEqual(cache.Get(1).Value, "Barry Allen");

            cache.Add(new MainCacheItem(4, " Avery Ho"));

            //Check if item is still in the cache
            Assert.AreEqual(cache.Get(1).Value, "Barry Allen");

            //Check the evicted item
            Assert.Throws<KeyNotFoundException>(() => cache.Get(2));
        }

        [Test]
        public void GetEvicetdItem()
        {
            var cacheSize = 3;
            var cache = new Cache<int, string, MainCacheItem>(cacheSize);

            MainCacheItem evicetdItem = null;
            cache.ItemEvicted += delegate (object sender, EvictedItemArgs<int, string> args)
            {
                evicetdItem = (MainCacheItem)args.Item;
            };

            cache.Add(new MainCacheItem(1, "Barry Allen"));
            cache.Add(new MainCacheItem(2, "Wally West"));
            cache.Add(new MainCacheItem(3, "Bart Allen"));

            //Get first item to be evicted
            Assert.AreEqual(cache.Get(1).Value, "Barry Allen");

            cache.Add(new MainCacheItem(4, " Avery Ho"));

            //Check if item is still in the cache
            Assert.AreEqual(cache.Get(1).Value, "Barry Allen");

            //Check the evicted item
            Assert.Throws<KeyNotFoundException>(() => cache.Get(2));

            Assert.AreEqual(evicetdItem?.Key, 2);
        }
    }
}