using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace PyroSquidUniLib.Encryption
{
    public class Hashing
    {
        private static readonly byte[] PepperBytes = Encoding.UTF8.GetBytes("YaWPxQ0h4Ax4JzWyyrPsai9DZ5dd9Tj9TNC2ZHmMYVFi1WzNQUn7BYvzGwYCHJeAPeH6yjaAAPldjSouZOxDPjwtRawtl27W5e6hs1iArFiSQp8EvH5Di4aJcUd5JP0Q");

        public static string ComputeHash(string text, HashAlgorithm algo, byte[] salt)
        {
            byte[] hash;

            var passwordBytes = Encoding.UTF8.GetBytes(text);

            var saltedPasswordBytes = new List<byte>();
            saltedPasswordBytes.AddRange(salt);
            saltedPasswordBytes.AddRange(passwordBytes);
            saltedPasswordBytes.AddRange(PepperBytes);

            using (var alg = algo)
            {
                hash = alg.ComputeHash(saltedPasswordBytes.ToArray());
            }

            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            var result = new StringBuilder();

            for (var i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            return result.ToString();
        }

        public static bool Confirm(string text, string hashValue)
        {
            var tempBool = false;

            var hashedL1 = hashValue;
            var hashedL2 = text;

            switch (hashedL1.SequenceEqual(hashedL2))
            {
                case true:
                    tempBool = true;
                    break;

                case false:
                    tempBool = false;
                    break;
            }

            return tempBool;
        }
    }
}