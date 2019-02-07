using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;

namespace Text
{
    public class Csv
    {
        private readonly static string[] _Delimiters = new string[] { "," };

        public bool HasHeaderRecord { get; set; }
        public Encoding encoding { get; set; }

        private string FilePath;

        public Csv()
        {
            HasHeaderRecord = true;
        }

        public Csv(string csvPath) : this()
        {
            FilePath = csvPath;
        }

        public Csv(string csvPath, Encoding encoding) : this(csvPath) { }

        public static DataTable Convert(Stream st, string tableName, DataColumn[] columns, DataColumn[] primaryKey, IDictionary<string, int> colIndexes, Encoding defaultEncoding)
        {
            var table = new DataTable(tableName);
            table.Columns.AddRange(columns);
            table.PrimaryKey = primaryKey;
            st.Seek(0, SeekOrigin.Begin);
            Convert(st, table, colIndexes, defaultEncoding);
            return table;
        }

        public static DataTable Convert(Stream st, string tableName, DataColumn[] columns, DataColumn[] primaryKey, IDictionary<string, int> colIndexes)
        {
            return Convert(st, tableName, columns, primaryKey, colIndexes, Encoding.UTF8);
        }

        public static void Convert(string csvTxt, DataTable orderTable, IDictionary<string, int> colIndexes, bool hasHeader = true)
        {
            using (var reader = new System.IO.StringReader(csvTxt))
            using (var tfp = new TextFieldParser(reader) { Delimiters = _Delimiters })
            {
                Convert(tfp, orderTable, colIndexes, hasHeader);
            }
        }

        public static void Convert(Stream stream, DataTable orderTable, IDictionary<string, int> colIndexes, Encoding defaultEncoding, bool hasHeader = true)
        {
            var tfp = new TextFieldParser(stream, defaultEncoding) { Delimiters = _Delimiters };
            Convert(tfp, orderTable, colIndexes, hasHeader);
        }

        public static void Convert(Stream stream, DataTable orderTable, IDictionary<string, int> colIndexes, bool hasHeader = true)
        {
            Convert(stream, orderTable, colIndexes, Encoding.UTF8, hasHeader);
        }

        public static void Convert(TextFieldParser tfp, DataTable orderTable, IDictionary<string, int> colIndexes, bool hasHeader)
        {
            // parse CSV
            var rowId = 0;
            while (!tfp.EndOfData)
            {
                string[] fields = tfp.ReadFields();
                if (hasHeader && ((++rowId) == 1))
                {
                    continue;
                }
                DataRow row = orderTable.NewRow();
                foreach (var colName in colIndexes.Keys)
                {
                    if(!String.IsNullOrEmpty(fields[colIndexes[colName]].ToString())){
                        row[colName] = fields[colIndexes[colName]];
                    }
                }
                orderTable.Rows.Add(row);
            }
        }

        /// <summary>
        /// DataTableの内容をCSVファイルに保存する
        /// </summary>
        /// <param name="dt">CSVに変換するDataTable</param>
        /// <param name="csvPath">保存先のCSVファイルのパス</param>
        /// <param name="writeHeader">ヘッダを書き込む時はtrue。</param>
        public void ConvertDataTableToCsv(DataTable dt, string csvPath, bool writeHeader)
        {
            //CSVファイルに書き込むときに使うEncoding
            var enc = System.Text.Encoding.GetEncoding("Shift_JIS");

            //書き込むファイルを開く
            using (var sr = new System.IO.StreamWriter(csvPath, false, enc))
            {
                int colCount = dt.Columns.Count;
                int lastColIndex = colCount - 1;

                //ヘッダを書き込む
                if (writeHeader)
                {
                    for (int i = 0; i < colCount; i++)
                    {
                        //ヘッダの取得
                        string field = dt.Columns[i].Caption;
                        //"で囲む
                        field = EncloseDoubleQuotesIfNeed(field);
                        //フィールドを書き込む
                        sr.Write(field);
                        //カンマを書き込む
                        if (lastColIndex > i)
                        {
                            sr.Write(',');
                        }
                    }
                    //改行する
                    sr.Write("\r\n");
                }

                //レコードを書き込む
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < colCount; i++)
                    {
                        //フィールドの取得
                        string field = row[i].ToString();
                        //"で囲む
                        field = EncloseDoubleQuotesIfNeed(field);
                        //フィールドを書き込む
                        sr.Write(field);
                        //カンマを書き込む
                        if (lastColIndex > i)
                        {
                            sr.Write(',');
                        }
                    }
                    //改行する
                    sr.Write("\r\n");
                }
            }
        }

        /// <summary>
        /// 必要ならば、文字列をダブルクォートで囲む
        /// </summary>
        private string EncloseDoubleQuotesIfNeed(string field)
        {
            if (NeedEncloseDoubleQuotes(field))
            {
                return EncloseDoubleQuotes(field);
            }
            return field;
        }

        /// <summary>
        /// 文字列をダブルクォートで囲む
        /// </summary>
        private string EncloseDoubleQuotes(string field)
        {
            if (field.IndexOf('"') > -1)
            {
                //"を""とする
                field = field.Replace("\"", "\"\"");
            }
            return "\"" + field + "\"";
        }

        /// <summary>
        /// 文字列をダブルクォートで囲む必要があるか調べる
        /// </summary>
        private bool NeedEncloseDoubleQuotes(string field)
        {
            return field.IndexOf('"') > -1 ||
                field.IndexOf(',') > -1 ||
                field.IndexOf('\r') > -1 ||
                field.IndexOf('\n') > -1 ||
                field.StartsWith(" ") ||
                field.StartsWith("\t") ||
                field.EndsWith(" ") ||
                field.EndsWith("\t");
        }

        public DataTable ToDataTable()
        {
            #region OleDbプロバイダを利用してテキストファイル(CSV)に接続する.
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
            using (DbConnection conn = factory.CreateConnection())
            {
                #region テキストファイルに接続する為の接続文字列を構築.
                //
                // 基本的にExcelに接続する場合とほぼ同じ要領となる。
                // Extended Properties内のISAMドライバがExcel 12.0からtextになる。
                // また、フォーマット方式を指定する必要がある。
                //
                // Data Sourceに指定するのは、該当ファイルが存在するディレクトリを指定する。
                // 尚、該当ファイルの構造については別途schema.iniファイルを同じディレクトリに
                // 用意する必要がある。
                //
                DbConnectionStringBuilder builder = factory.CreateConnectionStringBuilder();

                builder["Provider"] = "Microsoft.ACE.OLEDB.12.0";
                builder["Data Source"] = System.IO.Path.GetDirectoryName(FilePath);
                // // UTF8
                builder["Extended Properties"] = String.Format("text;CharacterSet=65001;HDR={0};FMT=Delimited", HasHeaderRecord ? "YES" : "NO");
                #endregion

                conn.ConnectionString = builder.ToString();
                conn.Open();

                //
                // SELECT.
                // FROM句の中に読み込む対象のファイル名を指定する。
                // データが取得される際にschema.iniファイルが参照され、列定義が行われる。
                //
                using (DbCommand command = conn.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM [{0}]", System.IO.Path.GetFileName(FilePath));

                    var table = new DataTable();
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                    return table;
                }
            }
            #endregion

        }
    }
}
