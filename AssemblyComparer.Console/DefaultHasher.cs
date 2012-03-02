using System;
using System.IO;
using System.Security.Cryptography;

namespace AssemblyComparer.Console
{
    internal class DefaultHasher : IHasher
    {
        public string GetHashForComparison(string sourcePath)
        {
            var byteArray = File.ReadAllBytes(sourcePath);
            var md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(byteArray);
            var hashString = BitConverter.ToString(result);
            return hashString;
        }
    }
}