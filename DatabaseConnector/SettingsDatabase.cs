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


namespace PMDCP.DatabaseConnector
{
    public class SettingsDatabase
    {
        public IDatabase Database { get; }

        public SettingsDatabase(IDatabase database) {
            Database = database;
        }

        public void SaveSetting(string table, string key, string value) {
            bool localConnection = false;
            if (Database.ConnectionState == System.Data.ConnectionState.Closed) {
                localConnection = true;
                Database.OpenConnection();
            }
            try {
                if (!Database.TableExists(table)) {
                    Database.CreateTable(table, new IDataField[] { Database.CreateField("Key", "TEXT"), Database.CreateField("Value", "TEXT") });
                }
                Database.UpdateOrInsert(table, new IDataColumn[] { Database.CreateColumn(true, "Key", key), Database.CreateColumn(false, "Value", value) }, "Key='" + key + "'");
            } finally {
                if (localConnection) {
                    Database.CloseConnection();
                }
            }
        }

        public string RetrieveSetting(string table, string key) {
            bool localConnection = false;
            if (Database.ConnectionState == System.Data.ConnectionState.Closed) {
                localConnection = true;
                Database.OpenConnection();
            }
            try {
                if (!Database.TableExists(table)) {
                    return null;
                }
                IDataColumn[] retrievedRow = Database.RetrieveRow(table, "Value", "Key='" + key + "'");
                if (retrievedRow != null) {
                    return (string)retrievedRow[0].Value;
                } else {
                    return null;
                }
            } finally {
                if (localConnection) {
                    Database.CloseConnection();
                }
            }
        }
    }
}
