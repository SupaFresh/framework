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

namespace PMDCP.DatabaseConnector.MySql
{
    public class DataColumnCollection
    {

        #region Constructors

        public DataColumnCollection(int columnCount) {
            Columns = new IDataColumn[columnCount];
        }

        #endregion Constructors

        #region Properties

        public IDataColumn[] Columns { get; }

        #endregion Properties

        #region Methods

        public void SetColumn(int columnIndex, IDataColumn column) {
            Columns[columnIndex] = column;
        }

        public IDataColumn FindByName(string name) {
            for (int i = 0; i < Columns.Length; i++) {
                if (Columns[i].Name == name) {
                    return Columns[i];
                }
            }
            return null;
        }

        public IDataColumn this[string name] {
            get {
                return FindByName(name);
            }
        }

        public IDataColumn this[int index] {
            get {
                return Columns[index];
            }
        }

        #endregion Methods
    }
}