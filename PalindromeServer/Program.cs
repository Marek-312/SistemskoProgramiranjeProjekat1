using System.Net;
using System.Text;

namespace PalindromeServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pokretanje servera....");

            Logger logger=new Logger();
            FileExplorer fileExplorer=new FileExplorer(logger);
            HTTPServer server=new HTTPServer(fileExplorer);
            server.Start(["http://localhost:5050/"]);
        }
    }
}