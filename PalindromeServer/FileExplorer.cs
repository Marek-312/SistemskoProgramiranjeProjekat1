using System.Net;
using System.Text;

namespace PalindromeServer
{
    internal sealed class FileExplorer
    {
        private readonly string rootDirectory;
        private readonly LRUCache cache=new LRUCache(kapacitet:5);
        private readonly Dictionary<string, object> inProgress=new Dictionary<string, object>();
        private readonly object progressLock=new object();
        private readonly Logger logger;
        private readonly SemaphoreSlim semaphore=new SemaphoreSlim(3);
        public FileExplorer(Logger logger)
        {
            this.rootDirectory=Directory.GetCurrentDirectory();
            this.logger=logger;
        }
        public static bool IsPalindrome(string word)
        {
            string cleaned=new string(word.Where(char.IsLetter).ToArray()).ToLowerInvariant();
            if(cleaned.Length<2)
            {
                return false;
            } 
            int left=0;
            int right=cleaned.Length-1;
            while(left<right)
            {
                if(cleaned[left]!=cleaned[right])
                {
                    return false;
                }
                left++;
                right--;
            }
            return true;
        }
        private static int CountPalindromes(string filePath)
        {
            Thread.Sleep(5000);//Dodato zbog testa
            int count=0;
            foreach(string line in File.ReadLines(filePath))
            {
                string[]words=line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                foreach(string w in words)
                {
                    if(IsPalindrome(w))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        private static void SendResponse(HttpListenerResponse response, int statusCode, string body)
        {
            byte[]buffer=Encoding.UTF8.GetBytes(body);
            response.StatusCode=statusCode;
            response.ContentType="text/plain; charset=utf-8";
            response.ContentLength64=buffer.Length;
            using Stream output=response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
        }
    }
}