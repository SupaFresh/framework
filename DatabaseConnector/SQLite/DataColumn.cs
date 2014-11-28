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

namespace PMU.DatabaseConnector.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DataColumn : IDataColumn
    {
        #region Fields

        int index;
        string name;
        object value;
        bool primaryKey;

        #endregion Fields

        #region Constructors

        public DataColumn(int index, bool primaryKey, string name, object value) {
            this.index = index;
            this.name = name;
            this.value = value;

            this.primaryKey = primaryKey;
        }

        public DataColumn(bool primaryKey, string name, object value) {
            this.index = -1;
            this.name = name;
            this.value = value;

            this.primaryKey = primaryKey;
        }

        #endregion Constructors

        #region Properties

        public bool PrimaryKey {
            get { return primaryKey; }
            set { primaryKey = value; }
        }

        public int Index {
            get { return index; }
        }

        public string Name {
            get {
                return name;
            }
            set {
                this.name = value;
            }
        }

        public object Value {
            get {
                return value;
            }
            set {
                this.value = value;
            }
        }

        public string ValueString {
            get {
                if (value != null) {
                    return value.ToString();
                } else {
                    return null;
                }
            }
        }

        #endregion Properties
    }
}