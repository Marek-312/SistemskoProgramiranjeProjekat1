internal sealed class Logger
{
    private readonly object lockObj=new object();

    public void Log(string poruka, string filename="unknown")
    {
        lock(lockObj)
        {
            string timeStamp=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            string entry=$"[{timeStamp}] [THREAD-{Thread.CurrentThread.ManagedThreadId}] [FAJL-{filename}] {poruka}";
            Console.WriteLine(entry);
            File.AppendAllText("log.txt", entry+"\n");
        
        }
    }
}