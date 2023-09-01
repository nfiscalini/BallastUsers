using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BL_MeterCheck_DataAccess.Extensions
{
    public static class DataConversionExtensions
    {
        #region Common

        private static readonly ConcurrentDictionary<Type, IList<PropertyInfo>> typeDictionary = new ConcurrentDictionary<Type, IList<PropertyInfo>>();

        private static IList<PropertyInfo> GetPropertiesForType<T>(DataTable table = null)
        {
            var type = typeof(T);
            if (!typeDictionary.ContainsKey(typeof(T)))
            {
                typeDictionary.TryAdd(type, type.GetProperties());
            }

            return typeDictionary[type];
        }

        #endregion

        #region DataRow Extensions

        public static T? To<T>(this DataRow row) where T : new()
        {
            if (row == null || row.Table.Columns.Count == 0)
            {
                return default;
            }
            var properties = GetPropertiesForType<T>(row.Table);

            return CreateItemFromRow<T>(row, properties);
        }

        public static T? To<T>(this DataRow row, Action<T, DataRow> action) where T : new()
        {
            if (row == null || row.Table.Columns.Count == 0)
                return default;

            var properties = GetPropertiesForType<T>(row.Table);
            T t = CreateItemFromRow<T>(row, properties);

            action?.Invoke(t, row);

            return t;
        }

        public static Dictionary<string, object> ToDictionary(this DataRow row)
        {
            if (row == null || row.Table.Columns.Count == 0)
                return new Dictionary<string, object>();

            var columns = row.Table.Columns;
            return CreateDictionaryFromRow(row, columns);
        }

        public static T Get<T>(this DataRow row, string columnName, T defaultValue = default)
        {
            return row?[columnName] == null || row[columnName] == DBNull.Value ? defaultValue : row.Get<T>(columnName);
        }

        public static T Get<T>(this DataRow row, int columnIndex, T defaultValue = default)
        {
            return columnIndex < 0 || row?[columnIndex] == null || row[columnIndex] == DBNull.Value ? defaultValue : row.Get<T>(columnIndex);
        }

        private static Dictionary<string, object> CreateDictionaryFromRow(this DataRow row, DataColumnCollection columns)
        {
            var item = new Dictionary<string, object>();
            for (var i = 0; i < columns.Count; i++)
            {
                var colName = columns[i].ColumnName;
                item.Add(colName, row[colName]);
            }

            return item;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            var item = new T();

            foreach (var property in properties.Where(p => p.CanWrite))
            {
                if (row.Table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                {
                    var t = property.PropertyType;
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        t = Nullable.GetUnderlyingType(property.PropertyType);
                    }
                    if (t.BaseType != typeof(System.Enum))
                        property.SetValue(item, Convert.ChangeType(row[property.Name], t), null);
                    else
                        property.SetValue(item, Enum.Parse(t, row[property.Name].ToString()), null);
                }
                else
                {
                    if (property.PropertyType == typeof(string) && property.GetValue(item) == null)
                        property.SetValue(item, "", null);
                }
            }
            return item;
        }

        #endregion

        #region DataTable Extensions

        public static IList<T> ToList<T>(this DataTable table, Action<T> action) where T : new()
        {
            var result = new List<T>();

            if (table?.Rows != null && table.Rows.Count != 0)
            {
                var properties = GetPropertiesForType<T>(table);

                foreach (var item in from object row in table.Rows select CreateItemFromRow<T>((DataRow)row, properties))
                {
                    result.Add(item);
                    action(item);
                }

                return result;
            }

            return result;
        }

        public static IList<T> ToList<T>(this DataTable table, Action<T, DataRow> action) where T : new()
        {
            var result = new List<T>();

            if (table?.Rows == null || table.Rows.Count == 0)
            {
                return result;
            }

            var properties = GetPropertiesForType<T>(table);

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                action(item, (DataRow)row);
                result.Add(item);
            }

            return result;
        }

        public static IList<Dictionary<string, object>> ToListDictionary(this DataTable table)
        {
            var result = new List<Dictionary<string, object>>();
            if (table?.Columns == null || table.Columns.Count == 0)
            {
                return result;
            }

            var columns = table.Columns;
            foreach (DataRow row in table.Rows)
            {
                result.Add(CreateDictionaryFromRow(row, columns));
            }
            return result;
        }

        public static IList<T> ToList<T>(this DataTable table) where T : new()
        {
            if (table?.Rows == null || table.Rows.Count == 0)
            {
                return new List<T>();
            }

            var properties = GetPropertiesForType<T>(table);
            return (from object row in table.Rows select CreateItemFromRow<T>((DataRow)row, properties)).ToList();
        }

        public static void Process<T>(this DataTable table, Action<T> action) where T : new()
        {
            if (table?.Rows == null || table.Rows.Count == 0)
            {
                return;
            }

            var properties = GetPropertiesForType<T>(table);
            foreach (var item in from object row in table.Rows select CreateItemFromRow<T>((DataRow)row, properties))
            {
                action(item);
            }
        }

        public static T One<T>(this DataTable table, T defaultValue = default) where T : new()
        {
            if (table == null || table.Rows.Count == 0)
            {
                return defaultValue;
            }

            return To<T>(table.Rows[0]);
        }

        public static Dictionary<string, object> OneAsDictionary(this DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return new Dictionary<string, object>();
            }

            return ToDictionary(table.Rows[0]);
        }

        public static void ForEachRow(this DataTable table, Action<DataRow> action)
        {
            if (table?.Rows == null || table.Rows.Count == 0)
            {
                return;
            }
            foreach (DataRow row in table.Rows)
            {
                action(row);
            }
        }

        public static DataRow GetDataRow(this DataTable table, int index)
        {
            if (table == null)
            {
                return new DataTable().NewRow();
            }
            if (table.Rows == null || table.Rows.Count == 0 || index < 0 || index >= table.Rows.Count)
            {
                return table.NewRow();
            }

            return table.Rows[index];
        }

        private static IList<T> F<T>(this DataTable table, Func<T, bool> filterFunc, bool skipOnFind = false) where T : new()
        {
            var result = new List<T>();

            if (table?.Rows == null || table.Rows.Count == 0)
            {
                return result;
            }

            var properties = GetPropertiesForType<T>(table);

            foreach (var item in from object row in table.Rows select CreateItemFromRow<T>((DataRow)row, properties))
            {

                if (!filterFunc(item))
                {
                    continue;
                }
                result.Add(item);
                if (skipOnFind)
                {
                    break;
                }
            }

            return result;
        }

        public static IList<T> Filter<T>(this DataTable table, Func<T, bool> filterFunc) where T : new()
        {
            return F<T>(table, filterFunc);
        }

        public static T Find<T>(this DataTable table, Func<T, bool> filterFunc) where T : new()
        {
            return F(table, filterFunc, true).FirstOrDefault();
        }

        private static DataTable ToDataTableComun(IReadOnlyList<PropertyInfo> properties, IEnumerable list)
        {
            var dataTable = new DataTable();
            foreach (var info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (var entity in list)
            {
                var values = new object[properties.Count];
                for (var i = 0; i < properties.Count; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static DataTable ToDataTable(this IEnumerable list, Type type)
        {
            if (list == null)
            {
                return new DataTable();
            }

            var properties = type.GetProperties();
            return ToDataTableComun(properties, list);
        }

        #endregion

        #region DataSet Extensions

        public static DataTable GetDataTable(this DataSet dataSet, string tableName)
        {
            var dt = new DataTable();
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[tableName] != null)
            {
                dt = dataSet.Tables[tableName];
            }
            return dt;
        }

        public static IList<T> GetList<T>(this DataSet dataSet, string tableName) where T : new()
        {
            return dataSet.GetDataTable(tableName).ToList<T>();
        }

        #endregion
    }
}
