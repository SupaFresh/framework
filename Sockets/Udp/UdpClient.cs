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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using PMDCP.Core;

namespace PMDCP.Sockets.Udp
{
    public class UdpClient
    {
        EndPoint bindEndPoint;

        public Socket Socket { get; }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public UdpClient() {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Send(byte[] data, string address, int port) {
            Send(data, IPAddress.Parse(address), port);
        }

        public void Send(byte[] data, IPAddress address, int port) {
            Send(data, new IPEndPoint(address, port));
        }

        public void Send(byte[] data, EndPoint endPoint) {
            Socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, endPoint, new AsyncCallback(SendCallback), this);
        }

        private void SendCallback(IAsyncResult result) {
            Socket.EndSendTo(result);
        }

        public void Listen(int port) {
            Listen(new IPEndPoint(IPAddress.Any, port));
        }

        public void Listen(EndPoint bindEndPoint) {
            this.bindEndPoint = bindEndPoint;
            byte[] recBuffer = new byte[256];
            Socket.Bind(bindEndPoint);
            Socket.BeginReceiveFrom(recBuffer, 0, recBuffer.Length,
                SocketFlags.None, ref bindEndPoint,
                new AsyncCallback(MessageReceivedCallback), recBuffer);

        }

        public void StartListenLoop(EndPoint bindEndPoint) {
            this.bindEndPoint = bindEndPoint;
            byte[] recBuffer = new byte[256];
            Socket.BeginReceiveFrom(recBuffer, 0, recBuffer.Length,
                SocketFlags.None, ref bindEndPoint,
                new AsyncCallback(MessageReceivedCallback), recBuffer);
        }

        private void MessageReceivedCallback(IAsyncResult result) {
            EndPoint remoteEndPoint = new IPEndPoint(0, 0);
            byte[] recBuffer = result.AsyncState as byte[];
            try {
                int bytesRead = Socket.EndReceiveFrom(result,
                    ref remoteEndPoint);
                DataReceived?.Invoke(this, new DataReceivedEventArgs(new ByteArray(recBuffer, bytesRead).ToString(), remoteEndPoint));
            } catch (SocketException e) {
                Console.WriteLine("Error: {0} {1}", e.ErrorCode, e.Message);
            }

            byte[] newBuffer = new byte[256];
            Socket.BeginReceiveFrom(newBuffer, 0, newBuffer.Length,
                SocketFlags.None, ref bindEndPoint,
                new AsyncCallback(MessageReceivedCallback), newBuffer);
        }
    }
}
