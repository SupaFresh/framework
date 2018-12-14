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

namespace PMDCP.DatabaseConnector.SQLite
{
    public class DataColumn : IDataColumn
    {
        #region Constructors

        public DataColumn(int index, bool primaryKey, string name, object value) {
            Index = index;
            Name = name;
            Value = value;

            PrimaryKey = primaryKey;
        }

        public DataColumn(bool primaryKey, string name, object value) {
            Index = -1;
            Name = name;
            Value = value;

            PrimaryKey = primaryKey;
        }

        #endregion Constructors

        #region Properties

        public bool PrimaryKey { get; set; }

        public int Index { get; }

        public string Name { get; set; }

        public object Value { get; set; }

        public string ValueString {
            get {
                if (Value != null) {
                    return Value.ToString();
                } else {
                    return null;
                }
            }
        }

        #endregion Properties
    }
}