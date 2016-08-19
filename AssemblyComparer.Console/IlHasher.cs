using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AssemblyComparer.Console
{
    public class IlHasher: IHasher
    {
        static readonly Regex MvidRegexp = new Regex("(?<=^//\\sMVID:\\s*)(?:(\\()|(\\{))?[A-Z0-9]{8}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12}(?(1)\\))(?(2)\\})", RegexOptions.Multiline);

        static readonly Regex ImageBaseRegexp = new Regex("(?<=^//\\sImage base:\\s*)[a-zA-Z0-9]*", RegexOptions.Multiline);

        static readonly Regex FooterRegexp = new Regex("^// \\**\\sDISASSEMBLY COMPLETE\\s[\\w\\W]*", RegexOptions.Multiline);

        static readonly Regex PrivateImplementationDetailsGuidRegexp = new Regex("(?<=<PrivateImplementationDetails>)(?:(\\()|(\\{))?[A-Z0-9]{8}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12}(?(1)\\))(?(2)\\})", RegexOptions.Multiline);

        static readonly Regex PrivateImplementationDetailsRegexp2 = new Regex("(?<='<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=[0-9]+' )([']?)?[A-Z0-9]{40}(([']?) at )(I_[0-9A-Z]*)", RegexOptions.Multiline);

        static readonly Regex PrivateImplementationDetailsDataCilRegexp = new Regex("(?<=.data cil )(I_[0-9A-Z]*)", RegexOptions.Multiline);

        public string GetHashForComparison(string sourcePath)
        {
            var il = Decompile(sourcePath);

            il = EraseBuildSpecificTokens(il);

            return GetMd5Hash(il);
        }

        private static string Decompile(string sourcePath)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "ildasm.exe";
            p.StartInfo.Arguments = string.Format("/raw /nobar /text {0}", sourcePath);
            p.Start();
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }

        private static string EraseBuildSpecificTokens(string il)
        {
            il = MvidRegexp.Replace(il, "{00000000-0000-0000-0000-000000000000}");
            il = ImageBaseRegexp.Replace(il, "0x00000000");
            il = PrivateImplementationDetailsGuidRegexp.Replace(il, "{00000000-0000-0000-0000-000000000000}");
            il = PrivateImplementationDetailsRegexp2.Replace(il, "'0000000000000000000000000000000000000000' at I_00000000");
            il = PrivateImplementationDetailsDataCilRegexp.Replace(il, "I_00000000");
            il = FooterRegexp.Replace(il, "");
            return il;
        }

        private static string GetMd5Hash(string il)
        {
            var md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(Encoding.Default.GetBytes(il));
            var hashString = BitConverter.ToString(result);
            return hashString;
        }
    }
}
