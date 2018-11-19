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

namespace PMDCP.Net
{
    public class FileDownloadingEventArgs : EventArgs
    {
        public FileDownloadingEventArgs(long fileSize, string filePath, int percent, long position) {
            this.FileSize = fileSize;
            this.FilePath = filePath;
            this.FileName = System.IO.Path.GetFileName(filePath).Replace(".tmp", "");
            this.Percent = percent;
            this.Position = position;
        }

        public long FileSize { get; }

        public string FileName { get; }

        public string FilePath { get; }

        public int Percent { get; }

        public long Position { get; }

    }
}
