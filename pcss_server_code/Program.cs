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
        static List<Thread> clientThreads = new List<Thread>();
        static LinkedList<Player> players = new LinkedList<Player>();
        static LinkedListNode<Player> activePlayerNode;


        public static void Main() {
            tcpListener.Start();
            Console.WriteLine("Starting server...");

            for (int i = 0; i < 3; i++) {
                Thread t = new Thread(new ThreadStart(Listener));
                t.Start();
                t.Name = "Player" + i;
                clientThreads.Add(Thread.CurrentThread);
            }

            Console.WriteLine("Client threads created, waiting for players to join...");

        }

        static void Listener() {

        }
    }
}
