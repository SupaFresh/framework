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

using System;
using System.Net;

namespace PMDCP.Sockets.Gnp
{
    public class GnpListener<TClientID>
    {
        private GnpClientCollection<TClientID> clientCollection;
        private IGnpIDGenerator<TClientID> idGenerator;
        private GnpClient gnpClient;

        public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;

        public GnpListener(IGnpIDGenerator<TClientID> idGenerator)
        {
            gnpClient = new GnpClient();
            this.idGenerator = idGenerator;

            Initialize();
        }

        private void Initialize()
        {
            clientCollection = new GnpClientCollection<TClientID>();
        }

        public void Listen(int port)
        {
            Listen(IPAddress.Any, port);
        }

        public void Listen(string ipAddress, int port)
        {
            Listen(IPAddress.Parse(ipAddress), port);
        }

        public void Listen(IPAddress ipAddress, int port)
        {
            Listen(new IPEndPoint(ipAddress, port));
        }

        public void Listen(EndPoint endPoint)
        {
            gnpClient.Listen(endPoint);

            gnpClient.DataReceived += new EventHandler<DataReceivedEventArgs>(gnpClient_DataReceived);
        }

        private void gnpClient_DataReceived(object sender, DataReceivedEventArgs e)
        {
            // We received data from a client that hasn't been redirected yet

            TClientID id = idGenerator.GenerateID(e);
            int index = clientCollection.IndexOf(id);

            GnpClient dataClient = null;
            if (index > -1)
            {
                dataClient = clientCollection[index];
            }
            else
            {
                dataClient = new GnpClient(e.DataSource);

                clientCollection.AddGnpClient(id, dataClient);

                ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs(id, dataClient));
            }

            dataClient.InjectData(e.ByteData, e.DataSource);
        }

        public void SendDataTo(byte[] data, TClientID clientID)
        {
            SendDataTo(data, clientCollection.GetGnpClient(clientID));
        }

        public void SendDataTo(byte[] data, GnpClient gnpClient)
        {
            this.gnpClient.Send(data, gnpClient.Socket.RemoteEndPoint);
        }

        public void SendDataToAll(byte[] data)
        {
            for (int i = 0; i < clientCollection.Count; i++)
            {
                SendDataTo(data, clientCollection[i]);
            }
        }
    }
}