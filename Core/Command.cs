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

using System.Collections.Generic;

namespace PMDCP.Core
{
    public class Command
    {
        #region Constructors

        internal Command(List<string> command)
        {
            CommandArgs = command;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the command line arguments for the program
        /// </summary>
        public List<string> CommandArgs { get; } = new List<string>();

        #endregion Properties

        #region Indexers

        public string this[int index] => CommandArgs[index];

        public string this[string argument]
        {
            get
            {
                int index = FindCommandArg(argument);
                if (index > -1 && CommandArgs.Count > index + 1)
                {
                    return CommandArgs[index + 1];
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Checks if a certain argument is included in the command line
        /// </summary>
        /// <param name="argToFind">The argument to look for</param>
        /// <returns>True if the argument exists; False if it doesn't exist.</returns>
        public bool ContainsCommandArg(string argToFind)
        {
            return CommandArgs.Contains(argToFind);
        }

        /// <summary>
        /// Retrieves the index of a certain argument in the command line.
        /// </summary>
        /// <param name="argToFind"></param>
        /// <returns>The index of the argument if it was found; otherwise, returns -1</returns>
        public int FindCommandArg(string argToFind)
        {
            return CommandArgs.IndexOf(argToFind);
        }

        #endregion Methods
    }
}