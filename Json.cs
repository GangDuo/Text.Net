using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Text
{
    public class Json
    {
        public static T Parse<T>(string jsonText)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            byte[] bytes = Encoding.UTF8.GetBytes(jsonText);
            using (var ms = new System.IO.MemoryStream(bytes))
            {
                return (T)serializer.ReadObject(ms);
            }
        }

        public static string ToString<T>(T o)
        {
            var json = new Json<T>(o);
            return json.Stringify();
        }
    }

    public class Json<T>
    {
        private T Source;

        public Json(T o)
        {
            Source = o;
        }

        public string Stringify()
        {
            using (var stream = new System.IO.MemoryStream())
            using (var sr = new StreamReader(stream))
            {
                StringifyThenRedirect(stream);
                stream.Position = 0;
                return sr.ReadToEnd();
            }
        }

        // 文字列のエンコーディングはUTF8
        public void StringifyThenRedirect(Stream w)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(w, Source);
        }

        public void StringifyThenRedirect(Stream w, Encoding encoding)
        {
            var writer = new StreamWriter(w, encoding);
            using (var ms = new MemoryStream())
            using (var reader = new StreamReader(ms))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(ms, Source);

                ms.Position = 0;
                // Sourceのサイズが大きい場合
                // 一回で書き込むとメモリ不足になるため分割処理
                var buf = new char[1024];
                while (reader.Peek() >= 0)
                {
                    var readSize = reader.Read(buf, 0, buf.Length);
                    if (readSize < 0) continue;
                    var characters = new char[readSize];
                    Array.Copy(buf, 0, characters, 0, readSize);
                    var text = new String(characters);
                    writer.Write(text);
                }
            }
            writer.Flush();
        }

    }
}
