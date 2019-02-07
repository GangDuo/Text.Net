using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class JsonUnitTests
    {
        private static readonly Dictionary<string, string> Dic = new Dictionary<string, string>()
            {
                {"k1","v1"},
                {"k2","v2"}
            };
        private static readonly string StringOfDic = "[{\"Key\":\"k1\",\"Value\":\"v1\"},{\"Key\":\"k2\",\"Value\":\"v2\"}]";

        [TestMethod]
        public void DictionaryToString()
        {
            var expected = StringOfDic;
            var actual = Text.Json.ToString(Dic);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void StringToDictionary()
        {
            var expected = Dic;
            var actual = Text.Json.Parse<Dictionary<string, string>>(StringOfDic);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ParseAndToString()
        {
            const string expected = @"[[1,2,3,4],[5,6,7,8],[9,10,11,12],[13,14,15,16],[17,18,19,20]]";

            var o = Text.Json.Parse<List<List<int>>>(expected);
            var actual = Text.Json.ToString(o);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ParseAndStringifyThenRedirect()
        {
            const string expected = @"[[1,2,3,4],[5,6,7,8],[9,10,11,12],[13,14,15,16],[17,18,19,20]]";
            var actual = String.Empty;

            var o = Text.Json.Parse<List<List<int>>>(expected);
            var json = new Text.Json<List<List<int>>>(o);
            using (var ms = new MemoryStream())
            {
                json.StringifyThenRedirect(ms);
                ms.Position = 0;
                actual = (new StreamReader(ms)).ReadToEnd();
            }
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ParseAndStringifyThenRedirectWithShiftJis()
        {
            const string expected = @"[[1,2,3,4],[5,6,7,8],[9,10,11,12],[13,14,15,16],[17,18,19,20]]";
            var actual = String.Empty;

            var o = Text.Json.Parse<List<List<int>>>(expected);
            var json = new Text.Json<List<List<int>>>(o);
            using (var ms = new MemoryStream())
            {
                json.StringifyThenRedirect(ms, Encoding.GetEncoding("Shift_JIS"));
                ms.Position = 0;
                actual = (new StreamReader(ms)).ReadToEnd();
            }
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        //[Ignore]
        public void TestMethod3()
        {
            var expected = new List<Option>()
            {
                new Option()
                {
                    ShopCode="001",
                    LineCodes=new string[]{""},
                    ItemCodes=new string[]{"208","214","215","217","218","250"},
                    SupplierCode="",
                    Date=DateTime.Parse("2014-08-26")
                },
                new Option()
                {
                    ShopCode="001",
                    LineCodes=new string[]{""},
                    ItemCodes=new string[]{"212","213","216","219","220","254","255","256"},
                    SupplierCode="",
                    Date=DateTime.Parse("2014-08-27")
                },
                new Option()
                {
                    ShopCode="001",
                    LineCodes=new string[]{""},
                    ItemCodes=new string[]{"209"},
                    SupplierCode="",
                    Date=DateTime.Parse("2014-08-28")
                },
            };
            var FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\stock_at.json");
            using (var sr = new StreamReader(FileName, Encoding.GetEncoding("shift_jis")))
            {
                string text = sr.ReadToEnd();
                sr.Close();
                var actual = Text.Json.Parse<List<Option>>(text);
                Assert.AreEqual(expected.Count, actual.Count);
                CollectionAssert.AreEqual(expected, actual, new OptionComparer());
            }
        }

        private class OptionComparer : IComparer, IComparer<Option>
        {
            public int Compare(object x, object y)
            {
                return Compare((Option)x, (Option)y);
            }

            public int Compare(Option x, Option y)
            {
                return x.Equals(y) ? 0 : 1;
            }
        }

        [DataContract]
        private class Option : IEquatable<Option>
        {
            [DataMember(Name = "shopCode")]
            public string ShopCode { get; set; }

            [DataMember(Name = "lineCodes")]
            public string[] LineCodes { get; set; }

            [DataMember(Name = "itemCodes")]
            public string[] ItemCodes { get; set; }

            [DataMember(Name = "supplierCode")]
            public string SupplierCode { get; set; }

            /* http://yasuand.hatenablog.com/entry/2013/09/12/051655 */
            [DataMember(Name = "date")]
            private string date_str_prop
            {
                get { return date_str_field; }
                set
                {
                    date_field = DateTime.ParseExact(value, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
                    date_str_field = value;
                }
            }
            private string date_str_field;
            public DateTime Date
            {
                get { return date_field; }
                set
                {
                    date_str_field = value.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                    date_field = value;
                }
            }
            private DateTime date_field { set; get; }

            public bool Equals(Option other)
            {
                if (other == null)
                {
                    return false;
                }

                return this.ShopCode.Equals(other.ShopCode)
                    && this.LineCodes.Except(other.LineCodes).ToList().Count == 0
                    && other.LineCodes.Except(this.LineCodes).ToList().Count == 0
                    && this.ItemCodes.Except(other.ItemCodes).ToList().Count == 0
                    && other.ItemCodes.Except(this.ItemCodes).ToList().Count == 0
                    && this.SupplierCode.Equals(other.SupplierCode)
                    && this.Date.Equals(other.Date);
            }
        }
    }
}
