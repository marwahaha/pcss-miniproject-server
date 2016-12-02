using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HandIn_Multithreading_Server2
{
    class Player
    {
        public int secretNumber;
        public int myTurn;
        public NetworkStream networkStream;
        public StreamWriter streamWriter;
        public StreamReader streamReader;
        public Socket clientSocket;
        public bool flag = true;

        public Player(Socket clientSocket)
        {
            this.clientSocket = clientSocket;

            Console.WriteLine("Client:" + clientSocket.RemoteEndPoint + " now connected to server.");
            networkStream = new NetworkStream(clientSocket);
            streamWriter = new StreamWriter(networkStream, Encoding.ASCII) { AutoFlush = true };
            streamReader = new StreamReader(networkStream, Encoding.ASCII);

            secretNumber = Int32.Parse(streamReader.ReadLine());
        }

        public void Disconnect()
        {
            streamReader.Close();
            networkStream.Close();
            streamWriter.Close();
            clientSocket.Close();
        }
    }
}
