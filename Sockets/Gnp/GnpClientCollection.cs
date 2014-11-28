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
using PMU.Core;

namespace PMU.Sockets.Gnp
{
    public class GnpClientCollection<TClientID>
    {
         ListPair<TClientID, GnpClient> clients;

         public GnpClientCollection() {
             clients = new ListPair<TClientID, GnpClient>();
        }

         public GnpClient GetGnpClient(TClientID clientID) {
            int index = clients.IndexOfKey(clientID);
            if (index > -1) {
                return clients.ValueByIndex(index);
            } else {
                return null;
            }
        }

         public int IndexOf(TClientID clientID) {
             return clients.IndexOfKey(clientID);
         }

         public ListPair<TClientID, GnpClient> Clients {
            get { return clients; }
        }

        public int Count {
            get { return clients.Count; }
        }

        public GnpClient this[TClientID clientID] {
            get { return clients.GetValue(clientID); }
        }

        public GnpClient this[int index] {
            get { return clients.ValueByIndex(index); }
        }

        public void AddGnpClient(TClientID clientID, GnpClient gnpClient) {
            if (clients.ContainsKey(clientID) == false) {
                // If the collection does not contain a client with the same ID, add it
                clients.Add(clientID, gnpClient);
            }
        }
    }
}
