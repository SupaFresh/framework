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
    using PMDCP.Core;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    public class TcpClient
    {
        #region Fields

        private const int BUFFER_SIZE = 1248;
        private ByteArray byteBuffer;
        private readonly byte[] packetCustomHeader;
        private bool inPacket;
        private string activeTransferFileName;
        private FileStream activeTransferFileStream;
        private MessageType packetType;
        private int totalPacketSize;
        private int receivedPacketSize;
        private readonly bool inFileTransfer;
        private bool buildingPacketData;

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public event EventHandler<FileTransferInitiationEventArgs> FileTransferInitiation;

        public event EventHandler ConnectionBroken;

        #endregion Fields

        #region Constructors

        public TcpClient(Socket socket)
        {
            Socket = socket;

            Initialize();

            StartReceivingLoop();
        }

        public TcpClient()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Initialize();
        }

        private void Initialize()
        {
            byteBuffer = new ByteArray();

            Socket.NoDelay = true;
        }

        #endregion Constructors

        #region Properties

        public Socket Socket { get; private set; }

        public TcpSocketState SocketState { get; private set; }

        public int CustomHeaderSize { get; set; }

        public object ID { get; internal set; }

        #endregion Properties

        #region Methods

        public void Close()
        {
            Socket.Close();
        }

        public void Connect(string ipAddressOrHostname, int port)
        {
            if (SocketState == TcpSocketState.Idle)
            {
                // Get host related information.
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddressOrHostname);

                SocketState = TcpSocketState.Connecting;

                // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
                // an exception that occurs when the host host IP Address is not compatible with the address family
                // (typical in the IPv6 case).
                TryNextTempSocketConnection(hostEntry.AddressList, 0, port);
            }
        }

        private void TryNextTempSocketConnection(IPAddress[] hostList, int currentIndex, int port)
        {
            if (currentIndex < hostList.Length)
            {
                IPAddress address = hostList[currentIndex];

                IPEndPoint endPoint = new IPEndPoint(address, port);
                Socket tempSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.BeginConnect(endPoint, new AsyncCallback(TempSocketConnectCallback), new object[] { tempSocket, hostList, currentIndex, port });
            }
            else
            {
                TempSocketConnectComplete();
            }
        }

        private void TempSocketConnectCallback(IAsyncResult result)
        {
            object[] state = result.AsyncState as object[];
            Socket tempSocket = state[0] as Socket;

            try
            {
                tempSocket.EndConnect(result);
            }
            catch { }
            if (tempSocket.Connected)
            {
                Socket = tempSocket;
                StartReceivingLoop();
                TempSocketConnectComplete();
            }
            else
            {
                IPAddress[] hostList = state[1] as IPAddress[];
                int currentIndex = (int)state[2];
                int port = (int)state[3];
                TryNextTempSocketConnection(hostList, currentIndex + 1, port);
            }
        }

        private void TempSocketConnectComplete()
        {
            SocketState = TcpSocketState.Idle;
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            if (SocketState != TcpSocketState.Connecting)
            {
                SocketState = TcpSocketState.Connecting;
                Socket.Connect(ipAddress, port);
                SocketState = TcpSocketState.Idle;
                //socket.BeginConnect(ipAddress, port, new AsyncCallback(ConnectCallback), null);

                StartReceivingLoop();
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            // We are connected!
            Socket.EndConnect(result);
            SocketState = TcpSocketState.Idle;
        }

        private void DataReceivedCallback(IAsyncResult result)
        {
            try
            {
                if (Socket != null && Socket.Connected)
                {
                    int packetSize = Socket.EndReceive(result);
                    if (packetSize > 0)
                    {
                        byte[] buffer = result.AsyncState as byte[];
                        ProcessReceivedPacket(new ByteArray(buffer, packetSize), packetSize);

                        //byte[] newBuffer = new byte[BUFFER_SIZE];
                        buffer = new byte[BUFFER_SIZE];
                        Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(DataReceivedCallback), buffer);
                    }
                    else
                    {
                        // Packet size is 0, so the connection was broken.
                        ConnectionBroken?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception)
            {
                //if (socket.Connected == false) {
                ConnectionBroken?.Invoke(this, EventArgs.Empty);
                //}
            }
        }

        private void ProcessReceivedPacket(ByteArray packetBuffer, int receivedPacketSize)
        {
            // Packet size is greater than 0, so a packet was received
            if (inPacket)
            {
                // We are still receiving data from a large packet
                int newPacketSize = this.receivedPacketSize + receivedPacketSize;
                if (newPacketSize >= totalPacketSize)
                {
                    if (packetType == MessageType.Generic)
                    {
                        // Append the remaining data
                        byteBuffer.AppendTo(packetBuffer);
                        this.receivedPacketSize += receivedPacketSize;
                        // Transfer the data to a new byte array
                        ByteArray newBuffer = new ByteArray(byteBuffer.ToArray());

                        inPacket = false;
                        totalPacketSize = 0;
                        this.receivedPacketSize = 0;
                        byteBuffer = null;

                        ProcessReceivedPacket(newBuffer, newPacketSize);
                    }
                    else if (packetType == MessageType.FileTransfer)
                    {
                        ByteArray fileBytes = packetBuffer.SubArray(0, totalPacketSize - this.receivedPacketSize);
                        if (activeTransferFileStream != null)
                        {
                            activeTransferFileStream.Write(fileBytes.ToArray(), 0, fileBytes.Length());

                            activeTransferFileStream.Close();
                            activeTransferFileStream.Dispose();
                            activeTransferFileStream = null;
                        }

                        inPacket = false;
                        totalPacketSize = 0;
                        this.receivedPacketSize = 0;
                        byteBuffer = null;

                        ByteArray newBuffer = packetBuffer.SubArray(fileBytes.Length(), packetBuffer.Length());
                        ProcessReceivedPacket(newBuffer, newBuffer.Length());
                    }
                }
                else
                {
                    if (packetType == MessageType.Generic)
                    {
                        byteBuffer.AppendTo(packetBuffer);
                    }
                    else if (packetType == MessageType.FileTransfer)
                    {
                        if (activeTransferFileStream != null)
                        {
                            activeTransferFileStream.Write(packetBuffer.ToArray(), 0, packetBuffer.Length());
                        }
                    }
                    this.receivedPacketSize += receivedPacketSize;
                }
            }
            else
            {
                if (IsValidPacket(packetBuffer))
                {
                    if (buildingPacketData)
                    {
                        byteBuffer.AppendTo(packetBuffer);
                        packetBuffer = new ByteArray(byteBuffer.ToArray());
                        buildingPacketData = false;
                        byteBuffer = null;
                    }
                    totalPacketSize = packetBuffer.SubArray(1, 5).ToInt() + GetHeaderSize();
                    packetType = (MessageType)packetBuffer[5];
                    byte[] packetCustomHeader = new byte[CustomHeaderSize];
                    for (int i = 0; i < CustomHeaderSize; i++)
                    {
                        packetCustomHeader[i] = packetBuffer[6 + i];
                    }

                    if (receivedPacketSize == totalPacketSize)
                    {
                        // We received the entire packet and nothing else
                        ByteArray fullPacket = packetBuffer.SubArray(GetHeaderSize(), totalPacketSize);

                        if (packetType == MessageType.Generic)
                        {
                            DataReceived?.Invoke(this, new DataReceivedEventArgs(fullPacket.ToArray(), packetCustomHeader, fullPacket.ToString()));
                        }
                        else if (packetType == MessageType.FileTransfer)
                        {
                            int fileNameSize = fullPacket.SubArray(0, 4).ToInt();
                            string fileName = fullPacket.SubArray(4, 4 + fileNameSize).ToString();

                            FileTransferInitiationEventArgs e = new FileTransferInitiationEventArgs(fileName);
                            FileTransferInitiation?.Invoke(this, e);

                            if (e.Accept)
                            {
                                byte[] fileBytes = fullPacket.SubArray(4 + fileNameSize, fullPacket.Length()).ToArray();

                                using (FileStream fileStream = new FileStream(e.DestinationDirectory + e.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    fileStream.Write(fileBytes, 0, fileBytes.Length);
                                }
                            }
                        }
                        inPacket = false;
                        totalPacketSize = 0;
                        this.receivedPacketSize = 0;
                        byteBuffer = null;
                    }
                    else if (receivedPacketSize > totalPacketSize)
                    {
                        // We received the entire packet and the start of another packet
                        ByteArray fullPacket = packetBuffer.SubArray(GetHeaderSize(), totalPacketSize);
                        ByteArray leftoverData = packetBuffer.SubArray(fullPacket.Length() + GetHeaderSize(), packetBuffer.Length());

                        if (packetType == MessageType.Generic)
                        {
                            DataReceived?.Invoke(this, new DataReceivedEventArgs(fullPacket.ToArray(), packetCustomHeader, fullPacket.ToString()));
                        }
                        else if (packetType == MessageType.FileTransfer)
                        {
                            int fileNameSize = fullPacket.SubArray(0, 4).ToInt();
                            string fileName = fullPacket.SubArray(4, 4 + fileNameSize).ToString();

                            FileTransferInitiationEventArgs e = new FileTransferInitiationEventArgs(fileName);
                            FileTransferInitiation?.Invoke(this, e);

                            if (e.Accept)
                            {
                                byte[] fileBytes = fullPacket.SubArray(4 + fileNameSize, fullPacket.Length()).ToArray();

                                using (FileStream fileStream = new FileStream(e.DestinationDirectory + e.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    fileStream.Write(fileBytes, 0, fileBytes.Length);
                                }
                            }
                        }

                        inPacket = false;
                        totalPacketSize = 0;
                        this.receivedPacketSize = 0;
                        byteBuffer = null;

                        ProcessReceivedPacket(leftoverData, leftoverData.Length());
                    }
                    else if (receivedPacketSize < totalPacketSize)
                    {
                        // We didn't receive the entire packet.
                        if (packetType == MessageType.Generic)
                        {
                            // If it's a generic packet, store it
                            byteBuffer = packetBuffer;
                        }
                        else if (packetType == MessageType.FileTransfer)
                        {
                            // If it's a file transfer, read the header and start writing
                            int fileNameSize = packetBuffer.SubArray(6, 10).ToInt();
                            activeTransferFileName = packetBuffer.SubArray(10, 10 + fileNameSize).ToString();

                            FileTransferInitiationEventArgs e = new FileTransferInitiationEventArgs(activeTransferFileName);
                            FileTransferInitiation?.Invoke(this, e);

                            if (e.Accept)
                            {
                                byte[] fileBytes = packetBuffer.SubArray(10 + fileNameSize, packetBuffer.Length()).ToArray();

                                activeTransferFileStream = new FileStream(e.DestinationDirectory + e.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                                activeTransferFileStream.Write(fileBytes, 0, fileBytes.Length);
                            }
                            else
                            {
                                activeTransferFileStream = null;
                            }
                        }
                        inPacket = true;
                        this.receivedPacketSize = receivedPacketSize;
                    }
                }
                else
                {
                    buildingPacketData = true;
                    byteBuffer = packetBuffer;
                    //ByteArray newBuffer = RepairPacketBuffer(packetBuffer);
                    //if (newBuffer != null) {
                    //    ProcessReceivedPacket(newBuffer, newBuffer.Length());
                    //}
                }
            }
        }

        public void Send(byte[] data, byte[] customHeader)
        {
            try
            {
                if (Socket.Connected)
                {
                    byte[] buffer = CreatePacketHeader(data.Length, MessageType.Generic, customHeader);

                    buffer = ByteEncoder.AppendToByteArray(buffer, data);
                    Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), buffer);
                }
            }
            catch
            {
                ConnectionBroken?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                if (Socket.Connected)
                {
                    byte[] buffer = CreatePacketHeader(data.Length, MessageType.Generic);
                    buffer = ByteEncoder.AppendToByteArray(buffer, data);

                    Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), buffer);
                }
            }
            catch
            {
                ConnectionBroken?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            try
            {
                Socket.EndSend(result);
            }
            catch
            {
                ConnectionBroken?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Listen(int port)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
            Socket.Bind(iep);
            Socket.Listen(10);
            Socket.BeginAccept(new AsyncCallback(ListenCallback), this);
        }

        private void ListenCallback(IAsyncResult result)
        {
            try
            {
                Socket client = Socket.EndAccept(result);
                Socket.Close();
                Socket = client;
                Socket.NoDelay = true;

                StartReceivingLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void StartReceivingLoop()
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(DataReceivedCallback), buffer);
        }

        public void SendFile(string filePath)
        {
            try
            {
                if (Socket.Connected)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    NetworkStream ns = new NetworkStream(Socket);
                    byte[] nameBytes = ByteEncoder.StringToByteArray(Path.GetFileName(filePath));
                    ByteArray header = new ByteArray(CreatePacketHeader(4 + nameBytes.Length + (int)fileInfo.Length, MessageType.FileTransfer));

                    FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                    int fullHeaderLength = header.Length() + 4 + nameBytes.Length;

                    int len = (int)fileInfo.Length;
                    byte[] b = new byte[fullHeaderLength];

                    header.ToArray().CopyTo(b, 0);
                    ByteEncoder.IntToByteArray(nameBytes.Length).CopyTo(b, header.Length());
                    nameBytes.CopyTo(b, header.Length() + 4);

                    int offset = fullHeaderLength;
                    int bytesRead = 0;
                    ns.Write(b, 0, b.Length);
                    //socket.Send(b, 0, b.Length, SocketFlags.None);
                    b = new byte[1024];
                    while (bytesRead < len)
                    {
                        bytesRead += file.Read(b, 0, (b.Length + bytesRead < len) ? b.Length : (len - bytesRead));
                        //socket.Send(b, 0, b.Length, SocketFlags.None);
                        //socket.Send(b, 0, b.Length, SocketFlags.None);
                        ns.Write(b, 0, b.Length);
                        if (offset != 0)
                        {
                            offset = 0;
                        }
                        b = new byte[1024];
                        file.Seek(bytesRead, SeekOrigin.Begin);
                    }
                    ns.Close();
                    ns.Dispose();
                }
            }
            catch
            {
            }
        }

        public int GetHeaderSize()
        {
            return
                1 // [byte] Constant indicating start of packet
                + 4 // [int32] Size of packet
                + 1 // [byte] Packet type

                + CustomHeaderSize // custom header
                ;
        }

        private byte[] CreatePacketHeader(int packetSize, MessageType messageType)
        {
            byte[] array = new byte[GetHeaderSize()];
            array[0] = 255;
            ByteEncoder.IntToByteArray(packetSize).CopyTo(array, 1);
            array[5] = (byte)messageType;
            return array;
        }

        private byte[] CreatePacketHeader(int packetSize, MessageType messageType, byte[] customHeader)
        {
            byte[] array = new byte[GetHeaderSize()];
            array[0] = 255;
            ByteEncoder.IntToByteArray(packetSize).CopyTo(array, 1);
            array[5] = (byte)messageType;
            int n = 6;
            for (int i = 0; i < customHeader.Length; i++)
            {
                if (n + i < array.Length)
                {
                    array[n + i] = customHeader[i];
                }
                else
                {
                    break;
                }
            }
            return array;
        }

        private bool IsValidPacket(ByteArray packetBuffer)
        {
            if (buildingPacketData == false)
            {
                if (packetBuffer.Length() <= 4 + GetHeaderSize())
                {
                    return false;
                }
                if (packetBuffer.IsEmpty(0, 4))
                {
                    return false;
                }
                return true;
            }
            else
            {
                if (byteBuffer.Length() + packetBuffer.Length() <= 4 + GetHeaderSize())
                {
                    return false;
                }
                return true;
            }
        }

        private ByteArray RepairPacketBuffer(ByteArray packetBuffer)
        {
            if (packetBuffer.IsEmpty())
            {
                return null;
            }
            else
            {
                int newStart = -1;
                for (int i = 0; i < packetBuffer.Length(); i++)
                {
                    if (packetBuffer[i] != 0)
                    {
                        newStart = i;
                        break;
                    }
                }
                if (newStart > -1)
                {
                    byte[] newArray = new byte[packetBuffer.Length() - newStart];
                    for (int i = newStart; i < packetBuffer.Length(); i++)
                    {
                        newArray[i - newStart] = packetBuffer[i];
                    }
                    //packetBuffer.ToArray().CopyTo(newArray, newStart);
                    return new ByteArray(newArray);
                }
                else
                {
                    return null;
                }
            }
        }

        public int LengthWithoutTrailingNulls(byte[] byteArray)
        {
            for (int i = byteArray.Length; i >= 0; i--)
            {
                if (byteArray[i] != 0)
                {
                    return i;
                }
            }
            return 0;
        }

        #endregion Methods
    }
}