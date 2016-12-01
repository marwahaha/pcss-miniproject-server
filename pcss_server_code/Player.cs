using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcss_server_code {

    class Player {

        public NetworkStream networkStream;
        public StreamWriter streamWriter;
        public StreamReader streamReader;
        public Socket clientSocket;

        public void Disconnect() {
            streamReader.Close();
            networkStream.Close();
            streamWriter.Close();
            clientSocket.Close();
        }
    }
}
