using Codeshell.Abp.Extensions;
using Codeshell.Abp.Importation;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Codeshell.Abp.Importation.Excel
{
    public class ExcelFile : IDisposable
    {
        public ExcelPackage Package { get; private set; }
        Dictionary<int, string[]> headers = new Dictionary<int, string[]>();
        Dictionary<int, Dictionary<int, ColumnTypes>> _columnTypes = null;
        public string[] DateFormatters { get; set; }

        public ExcelFile(string path, ExcelConfiguration config = null)
        {
            Package = new ExcelPackage();
            if (config != null)
            {
                _prepColumnTypes(config.Sheets);
                DateFormatters = config.DateFormatters?.ToArray();
            }

            if (!File.Exists(path))
                throw new FileNotFoundException(path);
            using (var stream = File.OpenRead(path))
            {
                Package.Load(stream);
            }
        }

        public ExcelFile(Stream stream)
        {
            Package = new ExcelPackage();
            Package.Load(stream);
        }

        public static ExcelFile FromResource(Assembly assembly, string key)
        {
            var resources = assembly.GetManifestResourceNames();
            var resource = resources.ToList().FirstOrDefault(e => e.Contains(key));
            if (resource == null)
            {
                throw new KeyNotFoundException($"Embedded file {key} not found in assembly '{assembly.FullName}'");
            }
            using (var resStream = assembly.GetManifestResourceStream(resource))
            {
                return new ExcelFile(resStream);
            }
        }

        private ColumnTypes? _getColumnType(int sheetId, int colId)
        {
            if (_columnTypes.TryGetValue(sheetId, out Dictionary<int, ColumnTypes> sheetDic))
            {
                if (sheetDic.TryGetValue(colId, out ColumnTypes type))
                    return type;
            }
            return null;
        }

        private void _prepColumnTypes(List<ExcelSheetConfig> cols)
        {
            _columnTypes = new Dictionary<int, Dictionary<int, ColumnTypes>>();
            for (var sheet = 0; sheet < cols.Count(); sheet++)
            {
                if (!_columnTypes.ContainsKey(sheet))
                    _columnTypes[sheet] = new Dictionary<int, ColumnTypes>();

                foreach (var c in cols[sheet].DateTimeCols)
                {
                    _columnTypes[sheet][c] = ColumnTypes.DateTime;
                }

                foreach (var c in cols[sheet].DecimalCols)
                {
                    _columnTypes[sheet][c] = ColumnTypes.Decimal;
                }

                foreach (var c in cols[sheet].LongCols)
                {
                    _columnTypes[sheet][c] = ColumnTypes.Long;
                }
            }
        }

        private string _initInsertQuery(string table_name, string[] headers)
        {
            string query = $"INSERT INTO {table_name} (";
            string comma = "";
            foreach (var h in headers)
            {
                var col = h.Replace(" ", "_").Replace(".", "");
                query += comma + col;
                comma = ",";
            }
            query += ") VALUES ";
            return query;
        }

        string _processValue(string v, ColumnTypes? type)
        {
            if (string.IsNullOrEmpty(v))
                return "null";
            else if (type != null)
            {
                v = v.Trim();
                switch (type.Value)
                {
                    case ColumnTypes.DateTime:
                        if (Utils.TryParseDate(v, DateFormatters, out DateTime date))
                            return $"'{date.ToString()}'";
                        else
                            return "null";
                    case ColumnTypes.Long:
                        if (long.TryParse(v, out long t))
                            return $"{t}";
                        else
                            return "null";
                    case ColumnTypes.Decimal:
                        if (decimal.TryParse(v, out decimal dec))
                            return $"{dec}";
                        else
                            return "null";
                    default:
                        return "N'" + v + "'";
                }

            }
            else
                return "N'" + v.Trim() + "'";
        }

        public string[] GetInsertCommands(int sheetId, string table_name, int limit = 500)
        {
            var headers = GetHeaders(sheetId);
            List<string> lst = new List<string>();
            lst.Add(_initInsertQuery(table_name, headers));
            int i = 0;
            int ii = 0;
            string commaLst = "";
            var ws = Package.Workbook.Worksheets[sheetId];
            Dictionary<int, ColumnTypes> dic = new Dictionary<int, ColumnTypes>();
            if (_columnTypes.ContainsKey(sheetId))
            {
                dic = _columnTypes[sheetId];
            }

            for (int rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
                lst[i] += commaLst + "\n(";
                commaLst = ",";
                var comma = "";
                for (int col = 0; col < ws.Dimension.End.Column; col++)
                {
                    var v = ws.Cells[rowNum, col + 1].Text;
                    ColumnTypes? colType = null;
                    if (dic.TryGetValue(col + 1, out ColumnTypes t))
                        colType = t;
                    lst[i] += comma + _processValue(v, colType);
                    comma = ",";
                }
                lst[i] += ")";
                if (ii >= limit)
                {
                    lst.Add(_initInsertQuery(table_name, headers));
                    i++;
                    ii = 0;
                    commaLst = "";
                }
                ii++;
            }
            return lst.ToArray();
        }

        public string GetTableCreationCommand(int sheetId, string table_name)
        {
            var headers = GetHeaders(sheetId);
            string creation = $"CREATE TABLE {table_name} (Id bigint PRIMARY KEY IDENTITY(1,1)";
            foreach (var h in headers)
            {
                var col = h.Replace(" ", "_").Replace(".", "");
                creation += ", " + col + " nvarchar(max)";
            }
            creation += ")";
            return creation;
        }



        public DataTable GetDataTable(int sheetId)
        {
            var ws = Package.Workbook.Worksheets[sheetId];
            var startRow = 2;
            DataTable lst = new DataTable();
            var heads = GetHeaders(sheetId);

            foreach (var h in heads)
                lst.Columns.Add();

            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
                var row = lst.Rows.Add();
                for (int col = 0; col < ws.Dimension.End.Column; col++)
                {
                    var val = ws.Cells[rowNum, col + 1].Value;
                    row[col] = val;
                }
            }
            return lst;
        }

        private string[] GetHeaders(int sheetId, int headerRow = 1)
        {
            string[] head = new string[0];
            if (headers.TryGetValue(sheetId, out head))
                return head;
            var ws = Package.Workbook.Worksheets[sheetId];
            List<string> lst = new List<string>();
            var rang = ws.Cells[headerRow, 1, 1, ws.Cells.End.Column];
            foreach (var r in rang)
                lst.Add(r.Text);
            head = lst.ToArray();
            headers[sheetId] = head;
            return head;
        }

        public List<T> GetListOf<T>(int sheetId, int headerRow = 1, int startFrom = 2) where T : class, IImportationRow
        {
            var headers = GetHeaders(sheetId, headerRow);
            Dictionary<string, int> mapping = new Dictionary<string, int>();
            int i = 1;
            foreach (var head in headers)
            {
                var propertyName = head.Replace("_", "").ToLower();
                var info = typeof(T).GetProperties().FirstOrDefault(e => e.Name.ToLower() == propertyName);
                if (info != null)
                {
                    mapping[info.Name] = i;
                }
                i++;
            }
            return GetListOf<T>(sheetId, mapping, startFrom);
        }

        public List<T> GetListOf<T>(int sheetId, Dictionary<string, int> mapping, int startRow = 2) where T : class, IImportationRow
        {

            Dictionary<int, List<PropertyInfo>> mappingDictionary = new Dictionary<int, List<PropertyInfo>>();
            foreach (var prop in mapping)
            {
                var inf = typeof(T).GetProperty(prop.Key);

                if (inf != null)
                {
                    if (!mappingDictionary.ContainsKey(prop.Value))
                    {
                        mappingDictionary[prop.Value] = new List<PropertyInfo>();

                    }
                    mappingDictionary[prop.Value].Add(inf);
                }

            }
            var ws = Package.Workbook.Worksheets[sheetId];

            List<T> lst = new List<T>();
            long id = 1;
            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
                T row = Activator.CreateInstance<T>();
                row.Id = id++;
                foreach (var item in mappingDictionary)
                {
                    var columnId = item.Key;
                    var properties = item.Value;
                    var val = ws.Cells[rowNum, columnId].Value;
                    foreach (var propertyInfo in properties)
                    {
                        object res = null;
                        if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                        {
                            if (Utils.TryParseDate(val?.ToString(), DateFormatters, out DateTime date))
                                res = date;
                        }
                        else if (propertyInfo.PropertyType == typeof(string))
                        {
                            if (!string.IsNullOrEmpty(val?.ToString()))
                                res = val.ToString();
                        }
                        else if (string.IsNullOrEmpty(val?.ToString()))
                        {
                            res = propertyInfo.PropertyType.GetDefaultValue();
                        }
                        else
                        {
                            res = Convert.ChangeType(val, propertyInfo.PropertyType);
                        }

                        propertyInfo.SetValue(row, res);
                    }

                }
                lst.Add(row);
            }
            return lst;
        }



        public void Dispose()
        {
            Package.Dispose();
        }
    }
}
