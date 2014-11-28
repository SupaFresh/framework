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

namespace PMU.DatabaseConnector
{
    public class SettingsDatabase
    {
        IDatabase database;

        public IDatabase Database {
            get { return database; }
        }

        public SettingsDatabase(IDatabase database) {
            this.database = database;
        }

        public void SaveSetting(string table, string key, string value) {
            bool localConnection = false;
            if (database.ConnectionState == System.Data.ConnectionState.Closed) {
                localConnection = true;
                database.OpenConnection();
            }
            try {
                if (!database.TableExists(table)) {
                    database.CreateTable(table, new IDataField[] { database.CreateField("Key", "TEXT"), database.CreateField("Value", "TEXT") });
                }
                database.UpdateOrInsert(table, new IDataColumn[] { database.CreateColumn(true, "Key", key), database.CreateColumn(false, "Value", value) }, "Key='" + key + "'");
            } finally {
                if (localConnection) {
                    database.CloseConnection();
                }
            }
        }

        public string RetrieveSetting(string table, string key) {
            bool localConnection = false;
            if (database.ConnectionState == System.Data.ConnectionState.Closed) {
                localConnection = true;
                database.OpenConnection();
            }
            try {
                if (!database.TableExists(table)) {
                    return null;
                }
                IDataColumn[] retrievedRow = database.RetrieveRow(table, "Value", "Key='" + key + "'");
                if (retrievedRow != null) {
                    return (string)retrievedRow[0].Value;
                } else {
                    return null;
                }
            } finally {
                if (localConnection) {
                    database.CloseConnection();
                }
            }
        }
    }
}
