using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class CsvUnitTests
    {
        [TestMethod]
        public void Convert()
        {
            // DataTable定義
            var fields = Enumerable.Repeat<string>("f", 3).Select((elm, i) => elm + ++i);
            var columns = new List<DataColumn>();
            foreach (var clm in fields)
            {
                columns.Add(new DataColumn(clm, Type.GetType("System.String")));
            }
            var actual = new DataTable();
            actual.Columns.AddRange(columns.ToArray());

            var colIndexes= new Dictionary<string,int>();
            foreach(var clm in fields.Select((elm, i) => Tuple.Create(elm, i)))
            {
                colIndexes.Add(clm.Item1, clm.Item2);
            };

            // テスト結果を設定
            var expected = actual.Clone();
            DataRow row = expected.NewRow();
            foreach (var key in colIndexes.Keys)
            {
                row[key] = colIndexes[key];
            }
            expected.Rows.Add(row);

            // テスト実行
            Text.Csv.Convert("0,1,2", actual, colIndexes, false);
            Assert.IsTrue(expected.AsEnumerable().SequenceEqual(actual.AsEnumerable(), DataRowComparer<DataRow>.Default));
        }

        [TestMethod]
        public void BuildDataTableFromCSV()
        {
            var csv = new Text.Csv(@"data\test.csv");
            var actual = csv.ToDataTable();
            Assert.AreEqual(3, actual.Rows.Count);
        }
    }
}
