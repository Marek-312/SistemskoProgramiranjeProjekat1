
namespace PalindromeServer
{
   /* internal sealed class LRUCache
    {
        private readonly int kapacitet;
        private readonly Dictionary<string, LinkedList<CacheItem>>map=new();

    }*/

    internal class CacheItem
    {
        public string FileName{get;}
        public int PalindromCount{get; set;}
        public CacheItem(string fileName, int count)
        {
            FileName=fileName;
            PalindromCount=count;
        }
    }
}