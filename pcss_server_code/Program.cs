using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace pcss_server_code {
    public class GameServer {
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
            Socket clientSocket = tcpListener.AcceptSocket();

            if (clientSocket.Connected) {
                Player player = new Player(clientSocket);
                LinkedListNode<Player> playerNode = new LinkedListNode<Player>(player);
                players.AddLast(playerNode);
                updatingPlayers.Add(player);

                if (activePlayerNode == null) activePlayerNode = playerNode;

                player.streamWriter.WriteLine("Lobby");
                player.myTurn = playerNumber;
                player.streamWriter.WriteLine(playerNumber);

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


                while (players.Count > 1) {


                    if (activePlayerNode.Value == player) {

                        if (activePlayerNode.Next != null)
                            nextNode = activePlayerNode.Next;
                        else
                            nextNode = players.First;

                        if (activePlayerNode.Previous != null)
                            prevNode = activePlayerNode.Previous;
                        else
                            prevNode = players.Last;

                        //On the players first turn, write which number player they are
                        if (activePlayerNode.Value.flag) {
                            activePlayerNode.Value.streamWriter.WriteLine("You are player number {0} out of {1}, and your secret number is {2}", activePlayerNode.Value.myTurn, players.Count, activePlayerNode.Value.secretNumber);
                            activePlayerNode.Value.flag = false;
                        }

                        activePlayerNode.Value.streamWriter.WriteLine("It's your turn!");
                        target = activePlayerNode.Value.streamReader.ReadLine();

                        if (target == "next") {
                            GuessNext();
                        }
                        if (target == "previous") {
                            GuessPrevious();
                        }

                        if (activePlayerNode.Next != null)
                            activePlayerNode = activePlayerNode.Next;
                        else
                            activePlayerNode = players.First;
                    }
                }

            }

            Console.WriteLine("Clients disconnected! : Press any key to shut down server...");
            Console.ReadKey();
        }

        static void GuessNext() {
            activePlayerNode.Value.streamWriter.WriteLine("Now guess!");
            guess = Int32.Parse(activePlayerNode.Value.streamReader.ReadLine());

            if (nextNode.Value.secretNumber == guess) {
                activePlayerNode.Value.streamWriter.WriteLine("It's a hit!");
                nextNode.Value.streamWriter.WriteLine("Game Over");
                nextNode.Value.Disconnect();
                players.Remove(nextNode);
            }

            else if (guess == 1000) {
                activePlayerNode.Value.streamWriter.WriteLine("change");
                activePlayerNode.Value.secretNumber = Int32.Parse(activePlayerNode.Value.streamReader.ReadLine());
            }

            else {
                if (nextNode.Value.secretNumber > guess)
                    activePlayerNode.Value.streamWriter.WriteLine("You missed. Try aiming higher next time. Wait for your turn.");
                else
                    activePlayerNode.Value.streamWriter.WriteLine("You missed. Try aiming lower next time. Wait for your turn.");
            }
        }

        static void GuessPrevious() {
            activePlayerNode.Value.streamWriter.WriteLine("Now guess!");
            guess = Int32.Parse(activePlayerNode.Value.streamReader.ReadLine());

            if (prevNode.Value.secretNumber == guess) {
                activePlayerNode.Value.streamWriter.WriteLine("It's a hit!");
                prevNode.Value.streamWriter.WriteLine("Game Over");
                prevNode.Value.Disconnect();
                players.Remove(prevNode);
            }

            else if (guess == 1000) {
                activePlayerNode.Value.streamWriter.WriteLine("change");
                activePlayerNode.Value.secretNumber = Int32.Parse(activePlayerNode.Value.streamReader.ReadLine());
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