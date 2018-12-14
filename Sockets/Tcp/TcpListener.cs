// This file is part of Mystery Dungeon eXtended.

// Mystery Dungeon eXtended is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Mystery Dungeon eXtended is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with Mystery Dungeon eXtended.  If not, see <http://www.gnu.org/licenses/>.

namespace PMDCP.Sockets.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public class TcpListener<TClientID>
    {

        #region Fields
        ITcpIDGenerator<TClientID> idGenerator;
        List<Socket> listenerSockets;

        #endregion Fields

        #region Constructors

        public TcpListener(ITcpIDGenerator<TClientID> idGenerator) {
            listenerSockets = new List<Socket>();

            this.idGenerator = idGenerator;

            Initialize();
        }

        #endregion Constructors

        #region Events

        public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;

        #endregion Events

        #region Properties

        public int Backlog { get; set; }

        public TcpClientCollection<TClientID> ClientCollection { get; private set; }

        #endregion Properties

        #region Methods

        public void Listen(string ipAddress, int port) {
            Listen(IPAddress.Parse(ipAddress), port);
        }

        public void Listen(IPAddress ipAddress, int port) {
            Listen(new IPEndPoint(ipAddress, port));
        }

        public void Listen(EndPoint endPoint) {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(endPoint);
            socket.Listen(Backlog);
            socket.BeginAccept(new AsyncCallback(ListenCallback), socket);

            listenerSockets.Add(socket);
        }

        public void SendDataTo(byte[] data, TClientID clientID) {
            SendDataTo(data, ClientCollection.GetTcpClient(clientID));
        }

        public void SendDataTo(byte[] data, TcpClient tcpClient) {
            tcpClient.Send(data);
        }

        public void SendDataToAll(byte[] data) {
            foreach (TcpClient client in ClientCollection.EnumerateAllClients()) {
                client.Send(data);
            }
        }

        private void Initialize() {
            Backlog = 10;
            ClientCollection = new TcpClientCollection<TClientID>();
        }

        private void ListenCallback(IAsyncResult result) {
            Socket socket = result.AsyncState as Socket;
            try {
                System.Net.Sockets.Socket client = socket.EndAccept(result);

                socket.BeginAccept(new AsyncCallback(ListenCallback), socket);

                TcpClient tcpClient = new TcpClient(client);
                TClientID id = idGenerator.GenerateID(tcpClient);
                ClientCollection.AddTcpClient(id, tcpClient);
                ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs(id, tcpClient));
            } catch {
                socket.BeginAccept(new AsyncCallback(ListenCallback), socket);
            }
        }

        #endregion Methods
    }
}