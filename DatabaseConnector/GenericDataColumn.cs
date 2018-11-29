﻿namespace PMDCP.DatabaseConnector
{
    public class GenericDataColumn : IGenericDataColumn
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string ValueString { get;}
        public int Index { get; set; }
        public bool PrimaryKey { get; set; }

        public GenericDataColumn(string name, bool primaryKey) {
            Name = name;
            PrimaryKey = primaryKey;
        }
    }
}
