using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Text
{
    public class Util
    {
        /**
         * テキストファイルを指定行数毎に分割する
         * 戻り値:
         *      分割後のテキストファイルパス
         */
        public static HashSet<string> Split(string path, uint linePerFile)
        {
            var ret = new HashSet<string>();
            var encoding = Encoding.GetEncoding("shift_jis");
            string[] lines = File.ReadAllLines(path, encoding);
            for (int cursor = 0, prefix = 0; cursor < lines.Length; ++prefix)
            {
                //ファイルを上書きし、Shift JISで書き込む
                var suffix = Path.GetFileName(path);
                var newPath = Path.Combine(Path.GetDirectoryName(path), String.Format("{0:000}", prefix) + suffix);
                ret.Add(newPath);
                using (TextWriter sw = new StreamWriter(
                    newPath,
                    false,
                    encoding))
                {
                    for (var i = 0; (i < linePerFile) && (cursor < lines.Length); ++i)
                    {
                        sw.WriteLine(lines[cursor++]);
                    }
                }
            }
            return ret;
        }

        public static string NewRandomStringLessThan32(int length)
        {
            Debug.Assert((length > 0) && (length <= 32));
            return Guid.NewGuid().ToString("N").Substring(0, length);
        }
    }
}
