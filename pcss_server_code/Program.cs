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
    public class GameServer
    {
        static TcpListener tcpListener = new TcpListener(IPAddress.Any, 1234);
        static List<Thread> clientThreads = new List<Thread>();
        static LinkedList<Player> players = new LinkedList<Player>();
        static List<Player> updatingPlayers = new List<Player>();
        static LinkedListNode<Player> activePlayerNode;
        static int playerNumber = 1;
        static int temp = 0;
        static int guess;
        static string target;
        static LinkedListNode<Player> nextNode;
        static LinkedListNode<Player> prevNode;

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
                Console.WriteLine("Point Reached 1");
                Player player = new Player(clientSocket);
                LinkedListNode<Player> playerNode = new LinkedListNode<Player>(player);
                players.AddLast(playerNode);
                updatingPlayers.Add(player);

                if (activePlayerNode == null) activePlayerNode = playerNode;

                player.streamWriter.WriteLine("Lobby");
                //The first player to connect gets a 1, then playNumber increments so the next player gets a 2 and so on
                Console.WriteLine("Point Reached 2");
                player.myTurn = playerNumber;
                player.streamWriter.WriteLine(playerNumber);

                Console.WriteLine("Point Reached 3");

                for (int i = 0; i < updatingPlayers.Count; i++) {
                    updatingPlayers[i].streamWriter.WriteLine("update");
                    temp = 3 - playerNumber;
                    updatingPlayers[i].streamWriter.WriteLine("Player " + playerNumber + "/3 connected. " + "Awaiting for " + temp + " player(s) to connect");
                }
                playerNumber++;



                // Wait for other players to connect/join
                while (players.Count != 3)
                    continue;

                player.streamWriter.WriteLine("Game Started");
            }

            Console.WriteLine("Clients disconnected! : Press any key to shut down server...");
            Console.ReadKey();
        }

        static void GuessNext()
        {
            activePlayerNode.Value.streamWriter.WriteLine("Now guess!");
            guess = Int32.Parse(activePlayerNode.Value.streamReader.ReadLine());

            if (nextNode.Value.secretNumber == guess)
            {
                activePlayerNode.Value.streamWriter.WriteLine("It's a hit!");
                nextNode.Value.streamWriter.WriteLine("Game Over");
                nextNode.Value.Disconnect();
                players.Remove(nextNode);
            }

            else if (guess == 1000)
            {
                activePlayerNode.Value.streamWriter.WriteLine("change");
                Console.WriteLine("Waiting reponds");
                activePlayerNode.Value.secretNumber = Int32.Parse(activePlayerNode.Value.streamReader.ReadLine());
                Console.WriteLine("skipped responds");
            }

            else
            {
                if (nextNode.Value.secretNumber > guess)
                    activePlayerNode.Value.streamWriter.WriteLine("You missed. Try aiming higher next time. Wait for your turn.");
                else
                    activePlayerNode.Value.streamWriter.WriteLine("You missed. Try aiming lower next time. Wait for your turn.");
            }
        }

        static void GuessPrevious()
        {
            activePlayerNode.Value.streamWriter.WriteLine("Now guess!");
            guess = Int32.Parse(activePlayerNode.Value.streamReader.ReadLine());

            if (prevNode.Value.secretNumber == guess)
            {
                activePlayerNode.Value.streamWriter.WriteLine("It's a hit!");
                prevNode.Value.streamWriter.WriteLine("Game Over");
                prevNode.Value.Disconnect();
                players.Remove(prevNode);
            }

            else if (guess == 1000)
            {
                activePlayerNode.Value.streamWriter.WriteLine("change");
                Console.WriteLine("Waiting reponds");
                activePlayerNode.Value.secretNumber = Int32.Parse(activePlayerNode.Value.streamReader.ReadLine());
                Console.WriteLine("skipped responds");
            }

            else {
                if (prevNode.Value.secretNumber > guess)
                    activePlayerNode.Value.streamWriter.WriteLine("You missed. Try aiming higher next time. Wait for your turn.");
                else
                    activePlayerNode.Value.streamWriter.WriteLine("You missed. Try aiming lower next time. Wait for your turn.");
            }
        }
    }
}
