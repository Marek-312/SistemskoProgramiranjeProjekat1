

using System.Net;
using System.Text;

namespace PalindromeServer
{
    internal sealed class HTTPServer
    {
        private readonly FileExplorer fileExplorer;
        private readonly RequestQueue requestQueue=new RequestQueue(5);

        public HTTPServer(FileExplorer fileExplorer)
        {
            this.fileExplorer=fileExplorer;
        }
        public void Start(string[] prefixes)
        {
            HttpListener listener=new HttpListener();
            
            foreach(string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
            listener.Start();
            Console.WriteLine("Server slusa");

            foreach(string prefix in prefixes)
            {
                Console.WriteLine($"{prefix}");
            }
            Thread consumerThread=new Thread(() =>
            {
                while(true)
                {
                    var context=requestQueue.Dequeue();
                    if(context==null)
                    {
                        break;
                    }
                    if(!ThreadPool.QueueUserWorkItem(fileExplorer.ThreadedFileSearch, context))
                    {
                        byte[] buffer=Encoding.UTF8.GetBytes("Server preopterecen");
                        context.Response.StatusCode=(int)HttpStatusCode.ServiceUnavailable;
                        context.Response.ContentLength64=buffer.Length;

                        using Stream output=context.Response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                    }
                }
            });
            consumerThread.IsBackground=true;
            consumerThread.Start();
            while(true)
            {
                try
                {
                    var context=listener.GetContext();
                    Console.WriteLine("Zahtev je primljen dodaje se u red");
                    requestQueue.Enqueue(context);
                }
                catch(HttpListenerException e)
                {
                    Console.WriteLine($"Listener greska {e.Message}");
                    break;
                }
                catch(ObjectDisposedException)
                {
                    break;
                }
            }
            listener.Stop();
        }
    }
}