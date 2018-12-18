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

namespace PMDCP.Core
{
    using System.Diagnostics;
    using System.Text;

    public class XCopy
    {
        #region Constructors

        public XCopy(string xcopyPath)
        {
            XCopyPath = xcopyPath;
        }

        #endregion Constructors

        #region Properties

        public string DestinationFile { get; set; }

        public bool PdbSearch { get; set; }

        public string SourceFile { get; set; }

        public string XCopyPath { get; }

        #endregion Properties

        #region Methods

        public void Copy()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(XCopyPath)
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            StringBuilder arguments = new StringBuilder();
            if (!string.IsNullOrEmpty(DestinationFile))
            {
                arguments.Append(" /dstfile \"" + DestinationFile + "\"");
            }
            if (!string.IsNullOrEmpty(SourceFile))
            {
                arguments.Append(" /srcfile \"" + SourceFile + "\"");
            }
            arguments.Append(" /pdbsearch " + PdbSearch);
            processStartInfo.Arguments = arguments.ToString();
            try
            {
                Process xcopyProcess = Process.Start(processStartInfo);
                xcopyProcess.WaitForExit();
            }
            catch { }
        }

        #endregion Methods
    }
}