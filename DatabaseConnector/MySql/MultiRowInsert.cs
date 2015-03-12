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

namespace PMDCP.DatabaseConnector.MySql
{
    public class MultiRowInsert
    {
        StringBuilder stringBuilder;
        bool firstRow;
        bool firstColumn;

        string[] columnNames;
        string tableName;

        int rowCount = 0;

        MySql database;
        bool hasData = false;

        public MultiRowInsert(MySql database, string tableName, params string[] columnNames) {
            this.database = database;
            this.tableName = tableName;
            this.columnNames = columnNames;

            UpdateParameters(tableName, columnNames);
        }

        private void PrepareForParameterUpdate() {
            stringBuilder = new StringBuilder();
            firstRow = true;
            firstColumn = true;
            hasData = false;
        }

        public void UpdateParameters(string tableName, string[] columnNames) {
            PrepareForParameterUpdate();

            this.tableName = tableName;
            this.columnNames = columnNames;

            stringBuilder.Append("REPLACE INTO `");
            stringBuilder.Append(tableName);
            stringBuilder.Append("` (");
            stringBuilder.Append(columnNames[0]);
            for (int i = 1; i < columnNames.Length; i++) {
                stringBuilder.Append(",");
                stringBuilder.Append(columnNames[i]);
            }
            stringBuilder.Append(") VALUES");
        }

        public void UpdateParameters(string tableName, string columnNames, string test) {
            PrepareForParameterUpdate();

            this.tableName = tableName;

            stringBuilder.Append("REPLACE INTO `");
            stringBuilder.Append(tableName);
            stringBuilder.Append("` (");
            stringBuilder.Append(columnNames);
            stringBuilder.Append(") VALUES");
        }

        public void AddRow(params string[] columnData) {
            AddRowOpening();
            foreach (string data in columnData) {
                AddColumnData(data);
            }
            AddRowClosing();
        }

        public void AddRowOpening() {
            if (stringBuilder == null) {
                UpdateParameters(tableName, columnNames);
            }

            if (!firstRow) {
                stringBuilder.Append(",");
            } else {
                firstRow = false;
            }
            stringBuilder.Append("(");
        }

        private void CheckFirstColumn() {
            hasData = true;
            if (!firstColumn) {
                stringBuilder.Append(",");
            } else {
                firstColumn = false;
            }
        }

        public void AddColumnData(string data) {
            CheckFirstColumn();
            stringBuilder.Append("\'");
            stringBuilder.Append(database.VerifyValueString(data));
            stringBuilder.Append("\'");
        }

        public void AddColumnData(params string[] data) {
            foreach (string dataString in data) {
                AddColumnData(dataString);
            }
        }

        public void AddColumnData(int data) {
            CheckFirstColumn();
            stringBuilder.Append(data);
        }

        public void AddColumnData(params int[] data) {
            foreach (int dataInt in data) {
                AddColumnData(dataInt);
            }
        }

        public void AddRowClosing() {
            stringBuilder.Append(")");
            firstColumn = true;

            rowCount++;
            if (rowCount >= 1000) {
                database.ExecuteNonQuery(GetSqlQuery());
                stringBuilder = null;
                rowCount = 0;
            }
        }

        public string GetSqlQuery() {
            if (stringBuilder != null && hasData == true) {
                return stringBuilder.ToString();
            } else {
                return "";
            }
        }

    }
}
