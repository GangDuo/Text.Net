using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Text.Extensions
{
    public static class Extensions // 拡張メソッドは非ジェネリック静的クラスで定義される必要がある
    {
        private static readonly Encoding SjisEnc = Encoding.GetEncoding("Shift_JIS");

        // 拡張メソッドは静的メソッドとして書く。第一引数にはthisキーワードをつける。
        public static string SingleQuote(this string v)
        {
            return Quote(v, "'");
        }

        public static string BackQuote(this string v)
        {
            return Quote(v, "`");
        }

        public static string DoubleQuote(this string v)
        {
            return Quote(v, "\"");
        }

        private static string Quote(this string v, string w)
        {
            return w + v + w;
        }

        public static bool IsZenkaku(this string v)
        {
            return SjisEnc.GetByteCount(v) == v.Length * 2;
        }

        public static bool HasZenkaku(this string v)
        {
            return !IsHankaku(v);
        }

        public static bool IsHankaku(this string v)
        {
            return SjisEnc.GetByteCount(v) == v.Length;
        }
        
        /// <summary>
        /// 文字列の先頭から指定した長さの文字列を取得する
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="len">長さ</param>
        /// <returns>取得した文字列</returns>
        public static string Left(this string str, int len)
        {
            if (len < 0)
            {
                throw new ArgumentException("引数'len'は0以上でなければなりません。");
            }
            if (str == null)
            {
                return "";
            }
            if (str.Length <= len)
            {
                return str;
            }
            return str.Substring(0, len);
        }

        /// <summary>
        /// 文字列の末尾から指定した長さの文字列を取得する
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="len">長さ</param>
        /// <returns>取得した文字列</returns>
        public static string Right(this string str, int len)
        {
            if (len < 0)
            {
                throw new ArgumentException("引数'len'は0以上でなければなりません。");
            }
            if (str == null)
            {
                return "";
            }
            if (str.Length <= len)
            {
                return str;
            }
            return str.Substring(str.Length - len, len);
        }
    }
}
