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

namespace PMU.DatabaseConnector.MySql
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DataColumnCollection
    {
        #region Fields

        IDataColumn[] columns;

        #endregion Fields

        #region Constructors

        public DataColumnCollection(int columnCount) {
            columns = new IDataColumn[columnCount];
        }

        #endregion Constructors

        #region Properties

        public IDataColumn[] Columns {
            get { return columns; }
        }

        #endregion Properties

        #region Methods

        public void SetColumn(int columnIndex, IDataColumn column) {
            columns[columnIndex] = column;
        }

        public IDataColumn FindByName(string name) {
            for (int i = 0; i < columns.Length; i++) {
                if (columns[i].Name == name) {
                    return columns[i];
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
                return columns[index];
            }
        }

        #endregion Methods
    }
}