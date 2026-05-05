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
    public HttpListenerContext? Dequeue()
    {
        lock(lockObj)
        {
            while(queue.Count==0)
            {
                if(completed)
                {
                    return null;
                }
                Console.WriteLine("[QUEUE] Red je prazna!");
                if(!Monitor.Wait(lockObj, TimeSpan.FromSeconds(30)))
                {
                    throw new Exception("Cekanje je predugo");
                }
            }
                var ctx=queue.Dequeue();
                Console.WriteLine("[QUEUE] Uzimam zahtev iz reda!");
                Monitor.PulseAll(lockObj);
                return ctx;
        }
    }
    public void Complete()
    {
        lock(lockObj)
        {
            completed=true;
            Monitor.PulseAll(lockObj);
        }
    }
}