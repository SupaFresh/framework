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

namespace PMDCP.Sockets
{
    using System.Text;

    using PMDCP.Core;

    public class TcpPacket : IPacket
    {
        #region Fields

        public const char SEP_CHAR = (char)0;
        bool inParamSegment;
        StringBuilder packet;
        long size = -1;

        #endregion Fields

        #region Constructors

        public TcpPacket(string header) {
            Header = header;
            packet = new StringBuilder(Header);
            packet.Append(SEP_CHAR);
        }

        public TcpPacket() {
            Header = null;
            packet = new StringBuilder();
        }

        #endregion Constructors

        #region Properties

        public ConnectionType ConnectionType {
            get { return ConnectionType.Tcp; }
        }

        public string Header { get; private set; }

        public string PacketString {
            get { return packet.ToString(); }
        }

        public char SeperatorChar {
            get { return SEP_CHAR; }
        }

        public long Size {
            get {
                if (size == -1) {
                    size = ByteEncoder.StringEncoding().GetByteCount(PacketString);
                }
                return size;
            }
        }

        public byte[] CustomHeader { get; set; }

        #endregion Properties

        #region Methods

        public static TcpPacket CreatePacket(string header, params string[] param) {
            TcpPacket packet = new TcpPacket(header);
            for (int i = 0; i < param.Length; i++) {
                packet.AppendParameter(param[i]);
            }
            return packet;
        }

        public static TcpPacket CreatePacket(string header, params int[] param) {
            TcpPacket packet = new TcpPacket(header);
            for (int i = 0; i < param.Length; i++) {
                packet.AppendParameter(param[i]);
            }
            return packet;
        }

        public static TcpPacket CreatePacket(string header) {
            TcpPacket packet = new TcpPacket(header);
            return packet;
        }

        public void AppendClass(ISendable sendableClass) {
            sendableClass.AppendToPacket(this);
        }

        public void AppendParameter(int parameter) {
            if (string.IsNullOrEmpty(Header)) {
                Header = parameter.ToString();
            }
            packet.Append(parameter.ToString());
            packet.Append(SEP_CHAR);
        }

        public void AppendParameter(string parameter) {
            if (string.IsNullOrEmpty(Header)) {
                Header = parameter;
            }
            packet.Append(parameter);
            packet.Append(SEP_CHAR);
        }

        public void AppendParameterSegment(string segment) {
            packet.Append(segment);
        }

        public void AppendParameters(params int[] param) {
            for (int i = 0; i < param.Length; i++) {
                packet.Append(param[i].ToString());
                packet.Append(SEP_CHAR);
            }
        }

        public void AppendParameters(params string[] param) {
            for (int i = 0; i < param.Length; i++) {
                packet.Append(param[i]);
                packet.Append(SEP_CHAR);
            }
        }

        public void EndParameterSegment() {
            packet.Append(SEP_CHAR);
            inParamSegment = false;
        }

        public void FinalizePacket() {
        }

        public void PrependParameters(params string[] param) {
            for (int i = param.Length - 1; i >= 0; i--) {
                packet.Insert(0, param[i]);
                packet.Insert(param[i].Length, SEP_CHAR);
            }
        }

        public void StartParameterSegment() {
            inParamSegment = true;
        }

        #endregion Methods
    }
}