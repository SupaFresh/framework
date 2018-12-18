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

namespace PMDCP.Core.IO
{
    public class Files
    {
        public static bool FileExists(string path)
        {
            return System.IO.File.Exists(path);
        }

        public static string GetFileSize(decimal byteCount)
        {
            string size = null;
            if (byteCount >= 1073741824)
            {
                size = string.Format("{0:##.##}", byteCount / 1073741824) + " GB";
            }
            else if (byteCount >= 1048576)
            {
                size = string.Format("{0:##.##}", byteCount / 1048576) + " MB";
            }
            else if (byteCount >= 1024)
            {
                size = string.Format("{0:##.##}", byteCount / 1024) + " KB";
            }
            else if (byteCount > 0 && byteCount < 1024)
            {
                size = byteCount.ToString() + " Bytes";
            }
            else
            {
                size = byteCount.ToString() + " Bytes";
            }

            return size;
        }
    }
}