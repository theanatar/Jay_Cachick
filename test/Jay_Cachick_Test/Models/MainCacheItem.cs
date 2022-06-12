using Jay_Cachick;

namespace Jay_Cachick_Test.Models
{
    public class MainCacheItem : CacheItem<int, string>
    {
        public MainCacheItem(int key, string value) : base(key, value)
        {
        }
    }
}
