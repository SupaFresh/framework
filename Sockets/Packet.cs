using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace PMU.Sockets
{
    class Packet : PMU.Sockets.IPacket
    {
        #region Fields

        public const char END_CHAR = (char)237;
        public const char SEP_CHAR = (char)0;

        string header;
        bool inParamSegment;
        StringBuilder packet;
        long size = -1;
        ConnectionType connectionType;

        #endregion Fields

        #region Constructors

        public Packet(string header) {
            this.header = header;
            packet = new StringBuilder(this.header);
            packet.Append(SEP_CHAR);

            connectionType = Sockets.ConnectionType.Tcp;
        }

        public Packet() {
            this.header = null;
            packet = new StringBuilder();

            connectionType = Sockets.ConnectionType.Tcp;
        }

        public Packet(ConnectionType connectionType) {
            this.header = null;
            packet = new StringBuilder();

            this.connectionType = connectionType;
        }

        public Packet(string header, ConnectionType connectionType) {
            this.header = header;
            packet = new StringBuilder(this.header);
            packet.Append(SEP_CHAR);

            this.connectionType = connectionType;
        }

        #endregion Constructors

        #region Properties

        public string Header {
            get { return header; }
        }

        public string PacketString {
            get { return packet.ToString(); }
        }

        public long Size {
            get {
                if (size == -1) {
                    size = ByteEncoder.StringEncoding().GetByteCount(PacketString);
                }
                return size;
            }
        }

        public ConnectionType ConnectionType {
            get { return connectionType; }
            set { connectionType = value; }
        }

        #endregion Properties

        #region Methods

        public static Packet CreatePacket(string header, params string[] param) {
            Packet packet = new Packet(header);
            for (int i = 0; i < param.Length; i++) {
                packet.AppendParameter(param[i]);
            }
            return packet;
        }

        public static Packet CreatePacket(string header, params int[] param) {
            Packet packet = new Packet(header);
            for (int i = 0; i < param.Length; i++) {
                packet.AppendParameter(param[i]);
            }
            return packet;
        }

        public static Packet CreatePacket(string header) {
            Packet packet = new Packet(header);
            return packet;
        }

        public void AppendClass(ISendable sendableClass) {
            sendableClass.AppendToPacket(this);
        }

        public void AppendParameter(string parameter) {
            if (string.IsNullOrEmpty(this.header)) {
                header = parameter;
            }
            packet.Append(parameter);
            packet.Append(SEP_CHAR);
        }

        public void AppendParameter(int parameter) {
            if (string.IsNullOrEmpty(this.header)) {
                header = parameter.ToString();
            }
            packet.Append(parameter.ToString());
            packet.Append(SEP_CHAR);
        }

        public void AppendParameterSegment(string segment) {
            packet.Append(segment);
        }

        public void AppendParameters(params string[] param) {
            for (int i = 0; i < param.Length; i++) {
                packet.Append(param[i]);
                packet.Append(SEP_CHAR);
            }
        }

        public void AppendParameters(params int[] param) {
            for (int i = 0; i < param.Length; i++) {
                packet.Append(param[i].ToString());
                packet.Append(SEP_CHAR);
            }
        }

        public void EndParameterSegment() {
            packet.Append(SEP_CHAR);
            inParamSegment = false;
        }

        public void PrependParameters(params string[] param) {
            for (int i = param.Length - 1; i >= 0; i--) {
                packet.Insert(0, param[i]);
                packet.Insert(param[i].Length, SEP_CHAR);
            }
        }

        public void FinalizePacket() {

        }

        public void StartParameterSegment() {
            inParamSegment = true;
        }

        #endregion Methods
    }
}
