using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Text
{
    public class RandomString
    {
        public class Options
        {
            public int Length { get; set; }
            public bool Numeric { get; set; }
            public bool Letters { get; set; }
            public bool Special { get; set; }

            public Options()
            {
                this.Length = 8;
                this.Numeric = true;
                this.Letters = true;
                this.Special = false;
            }
        }

        public static string Generate()
        {
            return Generate(new Options());
        }

        public static string Generate(Options options)
        {
            var rnd = "";
            var randomChars = BuildChars(options);

            for (var i = 1; i <= options.Length; i++)
            {
                byte[] randomNumber = new byte[1];
                rngCsp.GetBytes(randomNumber);
                var rn = randomNumber[0] % randomChars.Length;
                rnd += randomChars.Substring(rn, 1);
            }
            return rnd;
        }

        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private static readonly string Numbers = "0123456789";
        private static readonly string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static readonly string Specials = "!$%^&*()_+|~-=`{}[]:;<>?,./";

        private static string BuildChars(Options options)
        {
            var chars = "";
            if (options.Numeric) { chars += Numbers; }
            if (options.Letters) { chars += Letters; }
            if (options.Special) { chars += Specials; }
            return chars;
        }
    }
}
