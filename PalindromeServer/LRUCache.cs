
namespace PalindromeServer
{
    internal sealed class LRUCache
    {
        private readonly int kapacitet;
        private readonly Dictionary<string, LinkedListNode<CacheItem>>map=new();
        private readonly LinkedList<CacheItem>list=new();
        private readonly object cacheLock=new object();

        public LRUCache(int kapacitet)
        {
            this.kapacitet=kapacitet;
        } 

        public int Count
        {
            get{lock(cacheLock) {return map.Count;}}
        }

        public bool TryGet(string fileName, out int result)
        {
            lock(cacheLock)
            {
                if(map.TryGetValue(fileName, out var node))
                {
                    list.Remove(node);
                    list.AddFirst(node);
                    result=node.Value.PalindromCount;
                    return true;
                }
            }
            result=0;
            return false;
        }
         public void Set(string fileName, int palindromeCount)
        {
            lock(cacheLock)
            {
                if(map.TryGetValue(fileName, out var existing))
                {
                    existing.Value.PalindromCount=palindromeCount;
                    list.Remove(existing);
                    list.AddFirst(existing);
                    return;
                }
                if(map.Count>=kapacitet)
                {
                    var lru=list.Last!;
                    list.RemoveLast();
                    map.Remove(lru.Value.FileName);
                    Console.WriteLine($"[KES] izbacen je {lru.Value.FileName} zbog toga sto je kes pun!");
                }

                var item=new CacheItem(fileName, palindromeCount);
                var node=new LinkedListNode<CacheItem>(item);
                list.AddFirst(node);
                map[fileName]=node;
                Console.WriteLine($"[KES] Dodat je fajl {fileName} velicina kesa je: {map.Count}/{kapacitet}");
            }
        }
    }

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