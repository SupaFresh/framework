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

namespace PMDCP.Sockets.Gnp
{
    public class GnpClientCollection<TClientID>
    {
        public GnpClientCollection() {
             Clients = new ListPair<TClientID, GnpClient>();
        }

         public GnpClient GetGnpClient(TClientID clientID) {
            int index = Clients.IndexOfKey(clientID);
            if (index > -1) {
                return Clients.ValueByIndex(index);
            } else {
                return null;
            }
        }

         public int IndexOf(TClientID clientID) {
             return Clients.IndexOfKey(clientID);
         }

        public ListPair<TClientID, GnpClient> Clients { get; }

        public int Count {
            get { return Clients.Count; }
        }

        public GnpClient this[TClientID clientID] {
            get { return Clients.GetValue(clientID); }
        }

        public GnpClient this[int index] {
            get { return Clients.ValueByIndex(index); }
        }

        public void AddGnpClient(TClientID clientID, GnpClient gnpClient) {
            if (Clients.ContainsKey(clientID) == false) {
                // If the collection does not contain a client with the same ID, add it
                Clients.Add(clientID, gnpClient);
            }
        }
    }
}
