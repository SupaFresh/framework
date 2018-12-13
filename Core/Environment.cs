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
using System.IO;

namespace PMDCP.Core
{
    public class Environment
    {
        static DateTime compileDate = DateTime.MinValue;

        public static string StartupPath {
            get {
                return System.Reflection.Assembly.GetEntryAssembly().Location;
            }
        }

        public static string StartupDirectory {
            get {
                return System.IO.Path.GetDirectoryName(StartupPath);
            }
        }

        public static DateTime CompileDate {
            get {
                if (compileDate == DateTime.MinValue) {
                    compileDate = RetrieveLinkerTimestamp();
                }
                return compileDate;
            }
        }

        public static bool OnMono {
            get {
                Type t = Type.GetType("Mono.Runtime");
                if (t != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Retrieves the linker timestamp in UTC.
        /// </summary>
        /// <returns></returns>
        /// <remarks>http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html</remarks>
        private static System.DateTime RetrieveLinkerTimestamp() {
            string filePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;

            byte[] peHeader = new byte[2048];
            using (FileStream fileStream = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                fileStream.Read(peHeader, 0, 2048);
            }
            DateTime dt = new System.DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(System.BitConverter.ToInt32(peHeader, System.BitConverter.ToInt32(peHeader, peHeaderOffset) + linkerTimestampOffset));
            return dt;
        }
    }
}
