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

namespace PMU.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    public class XCopy
    {
        #region Fields

        string destinationFile;
        bool pdbSearch;
        string sourceFile;
        string xcopyPath;

        #endregion Fields

        #region Constructors

        public XCopy(string xcopyPath) {
            this.xcopyPath = xcopyPath;
        }

        #endregion Constructors

        #region Properties

        public string DestinationFile {
            get { return destinationFile; }
            set { destinationFile = value; }
        }

        public bool PdbSearch {
            get { return pdbSearch; }
            set { pdbSearch = value; }
        }

        public string SourceFile {
            get { return sourceFile; }
            set { sourceFile = value; }
        }

        public string XCopyPath {
            get { return xcopyPath; }
        }

        #endregion Properties

        #region Methods

        public void Copy() {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(xcopyPath);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            StringBuilder arguments = new StringBuilder();
            if (!string.IsNullOrEmpty(destinationFile)) {
                arguments.Append(" /dstfile \"" + destinationFile + "\"");
            }
            if (!string.IsNullOrEmpty(sourceFile)) {
                arguments.Append(" /srcfile \"" + sourceFile + "\"");
            }
            arguments.Append(" /pdbsearch " + pdbSearch);
            processStartInfo.Arguments = arguments.ToString();
            try {
                Process xcopyProcess = Process.Start(processStartInfo);
                xcopyProcess.WaitForExit();
            } catch { }
        }

        #endregion Methods
    }
}