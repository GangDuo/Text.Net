using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Text.Extensions;

namespace UnitTest
{
    [TestClass]
    public class ExtensionsUnitTests
    {
        private static readonly string _Word = "a b";
        private static readonly string _S = "'";
        private static readonly string _B = "`";
        private static readonly string _D = "\"";

        [TestMethod]
        public void SingleQuote()
        {
            var expected = _S + _Word + _S;
            Assert.AreEqual(expected, _Word.SingleQuote());
        }

        [TestMethod]
        public void BackQuote()
        {
            var expected = _B + _Word + _B;
            Assert.AreEqual(expected, _Word.BackQuote());
        }

        [TestMethod]
        public void DoubleQuote()
        {
            var expected = _D + _Word + _D;
            Assert.AreEqual(expected, _Word.DoubleQuote());
        }

        [TestMethod]
        public void IsZenkakuCaseZenkaku()
        {
            Assert.IsTrue("あい".IsZenkaku());
        }

        [TestMethod]
        public void IsZenkakuCaseAscii()
        {
            Assert.IsFalse("ab".IsZenkaku());
        }

        [TestMethod]
        public void IsZenkakuCaseMix()
        {
            Assert.IsFalse("あa".IsZenkaku());
        }

        [TestMethod]
        public void HasZenkaku()
        {
            Assert.IsTrue("あa".HasZenkaku());
            Assert.IsFalse("ab".HasZenkaku());
        }

        [TestMethod]
        public void IsHankaku()
        {
            Assert.IsFalse("あa".IsHankaku());
            Assert.IsTrue("ab".IsHankaku());
        }
    }
}
