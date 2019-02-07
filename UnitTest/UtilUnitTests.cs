using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class UtilUnitTests
    {
        [TestMethod]
        public void SplitNPieces()
        {
            Encoding ShiftJis = Encoding.GetEncoding("shift_jis");
            const uint linePerFile = 30;
            const string srcPath = @"data\100Lines.txt";
            var files = new List<string>(Text.Util.Split(srcPath, linePerFile));
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(linePerFile, (uint)File.ReadAllLines(files[i], ShiftJis).Length);
            }
            Assert.AreEqual(10, File.ReadAllLines(files[3], ShiftJis).Length);

            foreach (var path in files)
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void SplitWithoutFile()
        {
            Text.Util.Split(@"data\file_not_exists", 10);
        }

        [TestMethod]
        public void LowerLimitOfNewRandomStringLessThan32()
        {
            var limit = 1;
            Assert.IsTrue(limit == Text.Util.NewRandomStringLessThan32(limit).Length);
        }

        [TestMethod]
        public void UpperLimitOfNewRandomStringLessThan32()
        {
            var limit = 32;
            Assert.IsTrue(limit == Text.Util.NewRandomStringLessThan32(limit).Length);
        }
    }
}
