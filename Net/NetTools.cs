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
using System.Diagnostics;

using PMU.Core;
using System.Net.NetworkInformation;

namespace PMU.Net
{
    /// <summary>
    /// Misc. methods for communicating with the internet
    /// </summary>
    public class NetTools
    {
        #region Methods

        /// <summary>
        /// Opens a webpage in the default web browser.
        /// </summary>
        /// <param name="webpage">The webpage to open.</param>
        public static void OpenWebpage(string webpage) {
            OpenWebpage(new Uri(webpage));
        }

        /// <summary>
        /// Opens a webpage in the default web browser.
        /// </summary>
        /// <param name="webpage">The webpage to open.</param>
        public static void OpenWebpage(Uri website) {
            Process.Start(website.AbsolutePath);
        }

        public static bool IsConnected() {
            bool success = false;
            if (NetworkInterface.GetIsNetworkAvailable() == false) {
                return success;
            }
            string[] Mysite = { "www.pmuniverse.net" };
            try {
                using (Ping ping = new Ping()) {
                    foreach (string url in Mysite) {
                        PingReply replyMsg = ping.Send(url, 300);
                        if (replyMsg.Status == IPStatus.Success) {
                            success = true;
                            break;
                        }
                    }
                }
            } catch (Exception ex) {
            }
            return success;
        }

        public static string GetMacAddress() {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            for (int i = 0; i < nics.Length; i++) {
                if (i != NetworkInterface.LoopbackInterfaceIndex) {
                    return nics[i].GetPhysicalAddress().ToString();
                }
            }
            return null;
        }

        #endregion Methods

    }
}
