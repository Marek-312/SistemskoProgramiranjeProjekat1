using System.Net;
internal sealed class RequestQueue
{
    private readonly Queue<HttpListenerContext>queue=new Queue<HttpListenerContext>();
    private readonly int maxSize;
    private readonly object lockObj=new object();
    private bool completed=false;
    
    public RequestQueue(int maxSize)
    {
        this.maxSize=maxSize;
    }
    public void Enqueue(HttpListenerContext context)
    {
        lock(lockObj)
        {
            while(queue.Count>=maxSize)
            {
                Console.WriteLine("[QUEUE] Red je pun treba sacekati");
                Monitor.Wait(lockObj);
            }
            queue.Enqueue(context);
            Console.WriteLine("[QUEUE] dodat je zahtev u red");
            Monitor.PulseAll(lockObj);
        }

    }
}