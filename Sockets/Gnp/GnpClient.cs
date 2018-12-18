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

using PMDCP.Core;
using System;
using System.Net;
using System.Net.Sockets;

namespace PMDCP.Sockets.Gnp
{
    public class GnpClient
    {
        private const int MaxUDPSize = 0x10000;
        private readonly EndPoint destinationEndPoint;
        private bool listening = false;

        public Socket Socket { get; private set; }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public GnpClient(Socket socket)
        {
            Socket = socket;

            StartReceivingLoop();
        }

        public GnpClient(EndPoint destinationEndPoint)
        {
            this.destinationEndPoint = destinationEndPoint;
            Initialize();
            Connect(destinationEndPoint);
        }

        public GnpClient()
        {
            Initialize();
        }

        private void Initialize()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Listen(EndPoint endPoint)
        {
            Socket.Bind(endPoint);

            listening = true;
            StartReceivingLoop();
        }

        public void Listen(IPAddress ipAddress, int port)
        {
            IPEndPoint iep = new IPEndPoint(ipAddress, port);

            Listen(iep);
        }

        public void Listen(int port)
        {
            Listen(IPAddress.Any, port);
        }

        private void StartReceivingLoop()
        {
            byte[] buffer = new byte[MaxUDPSize];
            EndPoint tempRemoteEP;

            tempRemoteEP = new IPEndPoint(IPAddress.Any, 0);
            Socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref tempRemoteEP, new AsyncCallback(DataReceivedCallback), buffer);
        }

        private void Connect(EndPoint destinationEndPoint)
        {
            Socket.Connect(destinationEndPoint);

            StartReceivingLoop();
        }

        private void DataReceivedCallback(IAsyncResult result)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0); ;
            int bytesReceived = Socket.EndReceiveFrom(result, ref remoteEndPoint);

            byte[] buffer = (byte[])result.AsyncState;

            if (bytesReceived < MaxUDPSize)
            {
                byte[] newBuffer = new byte[bytesReceived];
                Buffer.BlockCopy(buffer, 0, newBuffer, 0, bytesReceived);

                ProcessIncomingData(newBuffer, remoteEndPoint);
            }

            IPEndPoint ipEndPoint = (IPEndPoint)remoteEndPoint;
            remoteEndPoint = new IPEndPoint(ipEndPoint.Address, 0);
            Socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEndPoint, new AsyncCallback(DataReceivedCallback), buffer);
        }

        public void Send(byte[] data, EndPoint remoteEndPoint)
        {
            Socket.SendTo(data, remoteEndPoint);
            //socket.SendTo(data, 0, data.Length, SocketFlags.None, remoteEndPoint, new AsyncCallback(SendCallback), buffer);
        }

        public void Send(byte[] data)
        {
            Send(data, destinationEndPoint);
        }

        private void ProcessIncomingData(byte[] data, EndPoint remoteEndPoint)
        {
            string val = ByteEncoder.ByteArrayToString(data);

            DataReceived?.Invoke(this, new DataReceivedEventArgs(data, null, val, remoteEndPoint));
        }

        public void InjectData(byte[] data, EndPoint remoteEndPoint)
        {
            ProcessIncomingData(data, remoteEndPoint);
        }
    }
}