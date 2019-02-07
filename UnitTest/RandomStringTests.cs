using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace UnitTest
{
    /// <summary>
    /// RandomStringTest の概要の説明
    /// </summary>
    [TestClass]
    public class RandomStringTests
    {
        public RandomStringTests()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void DefaultRandomStringLength()
        {
            Assert.AreEqual(8, Text.RandomString.Generate().Length);
        }

        [TestMethod]
        public void OptionalLength()
        {
            Assert.AreEqual(64, Text.RandomString.Generate(new Text.RandomString.Options() { Length = 64 }).Length);
        }

        [TestMethod]
        public void NumericOnly()
        {
            var testData = Text.RandomString.Generate(new Text.RandomString.Options() { Numeric = true, Letters = false, Special = false });
            var mc = Regex.Matches(testData, @"\D", RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
            Assert.AreEqual(0, mc.Count);
        }

        [TestMethod]
        public void LetterOnly()
        {
            var testData = Text.RandomString.Generate(new Text.RandomString.Options() { Numeric = false, Letters = true, Special = false });
            var mc = Regex.Matches(testData, @"[^a-zA-Z]", RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
            Assert.AreEqual(0, mc.Count);
        }

        [TestMethod]
        public void SpecialOnly()
        {
            var testData = Text.RandomString.Generate(new Text.RandomString.Options() { Numeric = false, Letters = false, Special = true });
            var mc = Regex.Matches(testData, @"[0-9a-zA-Z\s]", RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
            Assert.AreEqual(0, mc.Count);
        }
    }
}
