using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

public static class DataTableMapper
{
    public static List<T> MapDataTableToObjectList<T>(DataTable dataTable) where T : new()
    {
        List<T> objects = new List<T>();

        foreach (DataRow row in dataTable.Rows)
        {
            T obj = new T();

            foreach (DataColumn col in dataTable.Columns)
            {
                PropertyInfo property = obj.GetType().GetProperty(col.ColumnName);
                if (property != null && row[col] != DBNull.Value)
                {
                    Type propertyType = property.PropertyType;
                    object value = Convert.ChangeType(row[col], propertyType);
                    property.SetValue(obj, value, null);
                }
            }

            objects.Add(obj);
        }

        return objects;
    }

    public static T MapDataRowToObject<T>(DataTable dataTable) where T : new()
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
        {
            return default(T); // Return null or the default value for T
        }

        T obj = new T();

        foreach (DataRow row in dataTable.Rows)
        {
            foreach (DataColumn col in row.Table.Columns)
            {
                PropertyInfo property = obj.GetType().GetProperty(col.ColumnName);
                if (property != null && row[col] != DBNull.Value)
                {
                    Type propertyType = property.PropertyType;
                    object value = Convert.ChangeType(row[col], propertyType);
                    property.SetValue(obj, value, null);
                }
            }
        }

        return obj;
    }

}
