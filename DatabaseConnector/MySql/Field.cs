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
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Field : IDataField
    {
        #region Constructors

        public Field(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public Field(string name, FieldType type)
        {
            Name = name;
            switch (type) {
                case FieldType.Text:
                    Type = "TEXT";
                    break;
                case FieldType.Numeric:
                    Type = "NUMERIC";
                    break;
                case FieldType.Blob:
                    Type = "BLOB";
                    break;
                case FieldType.IntegerPrimaryKey:
                    Type = "INTEGER PRIMARY KEY";
                    break;
            }
        }

        #endregion Constructors

        #region Properties

        public string Name { get; set; }

        public string Type { get; set; }

        #endregion Properties
    }
}