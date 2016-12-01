using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcss_server_code
{
    class Program
    {
        static TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 1234);
        static bool isOnline;
        static int hej = 0;

        static void Main()
        {
            tcpListener.Start();
            Console.WriteLine("Starting server...");

         
        }
    }
}
