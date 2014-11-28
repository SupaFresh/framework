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
using PMU.Core;

namespace PMU.Sockets.Tcp
{
    public class TcpClientCollection<TClientID>
    {
        Object lockObject = new object();
        Dictionary<TClientID, TcpClient> clients;

        public TcpClientCollection() {
            clients = new Dictionary<TClientID, TcpClient>();
        }

        public TcpClient GetTcpClient(TClientID clientID) {
            lock (lockObject) {
                TcpClient tcpClient = null;
                if (clients.TryGetValue(clientID, out tcpClient)) {
                    return tcpClient;
                } else {
                    return null;
                }
            }
        }

        public IEnumerable<TcpClient> EnumerateAllClients() {
            lock (lockObject) {
                foreach (TcpClient client in clients.Values) {
                    yield return client;
                }
            }
        }

        public Dictionary<TClientID, TcpClient> Clients {
            get { return clients; }
        }

        public int Count {
            get { return clients.Count; }
        }

        public TcpClient this[TClientID clientID] {
            get { return GetTcpClient(clientID); }
        }
        
        public void AddTcpClient(TClientID clientID, TcpClient tcpClient) {
            lock (lockObject) {
                if (clients.ContainsKey(clientID) == false) {
                    // If the collection does not contain a client with the same ID, add it
                    clients.Add(clientID, tcpClient);
                    tcpClient.ConnectionBroken += new EventHandler(tcpClient_ConnectionBroken);
                }
            }
        }

        void tcpClient_ConnectionBroken(object sender, EventArgs e) {
            lock (lockObject) {
                TClientID clientID = (TClientID)((TcpClient)sender).ID;
                if (clientID != null && clients.ContainsKey(clientID)) {
                    clients.Remove(clientID);
                }
            }
        }
    }
}
