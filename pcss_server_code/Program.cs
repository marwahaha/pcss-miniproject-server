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
       // static LinkedList<Player> players = new LinkedList<Player>();
       // static LinkedListNode<Player> activePlayerNode;


        public static void Main()
        {
            tcpListener.Start();
            Console.WriteLine("Starting server...");

            //Loop for creating 3 threads for 3 players
            for (int i = 0; i < 3; i++)
            {
                Thread t = new Thread(new ThreadStart(Listener));
                t.Start();
                t.Name = "Player" + i;
                clientThreads.Add(Thread.CurrentThread);
            }

            Console.WriteLine("Client threads created, waiting for players to join...");

        }

        static void Listener() {
            Socket clientSocket = tcpListener.AcceptSocket();

            if (clientSocket.Connected) {
                Player player = new Player(clientSocket);
                LinkedListNode<Player> playerNode = new LinkedListNode<Player>(player);
                players.AddLast(playerNode);

                if (activePlayerNode == null) activePlayerNode = playerNode;

                //The first player to connect gets a 1, then playNumber increments so the next player gets a 2 and so on
                player.myTurn = playerNumber;
                playerNumber++;

                // Wait for other players to connect/join
                while (players.Count != 3) {
                    activePlayerNode.Value.streamWriter.WriteLine("{0} out of 3 player(s) are connected", players.Count);
                    continue;
                }
            }

            Console.WriteLine("Clients disconnected! : Press any key to shut down server...");
            Console.ReadKey();
        }
    }
}
