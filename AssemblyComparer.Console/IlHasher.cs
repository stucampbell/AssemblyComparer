using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AssemblyComparer.Console
{
    public class IlHasher: IHasher
    {
        /// <summary>
        /// This regex prequalifies a decompiled line for further processing (anonymization) by
        /// regexes in the Anonymization regexes region
        /// 
        /// IMPORTANT: This regest MUST always be maintained when adding new regexes so that
        /// the following holds:
        ///   If a line matches any of the Anonymization regexes, then it must also match
        ///   the PotentiallyUnsafeLineRegex
        /// </summary>
        static readonly Regex PotentiallyUnsafeLineRegex = new Regex("(MVID|Image base|DISASSEMBLY COMPLETE|PrivateImplementationDetails|data cil|ver)", RegexOptions.Multiline);

    #region  Anonymization regexes

        static readonly Regex MvidRegexp = new Regex("(?<=^//\\sMVID:\\s*)(?:(\\()|(\\{))?[A-Z0-9]{8}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12}(?(1)\\))(?(2)\\})", RegexOptions.Multiline);

        static readonly Regex ImageBaseRegexp = new Regex("(?<=^//\\sImage base:\\s*)[a-zA-Z0-9]*", RegexOptions.Multiline);

        static readonly Regex FooterRegexp = new Regex("^// \\**\\sDISASSEMBLY COMPLETE\\s[\\w\\W]*", RegexOptions.Multiline);

        static readonly Regex PrivateImplementationDetailsGuidRegexp = new Regex("(?<=<PrivateImplementationDetails>)(?:(\\()|(\\{))?[A-Z0-9]{8}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12}(?(1)\\))(?(2)\\})", RegexOptions.Multiline);

        static readonly Regex PrivateImplementationDetailsRegexp2 = new Regex("(?<='<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=[0-9]+' )([']?)?[A-Z0-9]{40}(([']?) at )(I_[0-9A-Z]*)", RegexOptions.Multiline);

        static readonly Regex PrivateImplementationDetailsDataCilRegexp = new Regex("(?<=.data cil )(I_[0-9A-Z]*) = bytearray", RegexOptions.Multiline);

        static readonly Regex ExtraDataCilRegexp = new Regex(".data cil I_[0-9A-Z]* = int8", RegexOptions.Multiline);

        static readonly Regex ReferenceVersionNumberRegexp = new Regex("(?<=.ver )([a-zA-Z0-9]+\\:){3}[a-zA-Z0-9]+", RegexOptions.Multiline);

    #endregion

        public string GetHashForComparison(string sourcePath)
        {
            var decompilationProcess = RunDecompilationProcess(sourcePath);
            var decompiledStream = decompilationProcess.StandardOutput;
            var sb = new StringBuilder();
            var i = 0;
            do
            {
                var rawLine = decompiledStream.ReadLine();
                i++;
                EraseBuildSpecificTokens(sb, rawLine);
            } while (!decompiledStream.EndOfStream);

            decompilationProcess.WaitForExit();

            return GetMd5Hash(sb.ToString().Trim());
        }

        private Process RunDecompilationProcess(string sourcePath)
        {
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    FileName = "ildasm.exe",
                    Arguments = $"/raw /nobar /text {sourcePath}"
                }
            };
            p.Start();
            return p;
        }

        private static void EraseBuildSpecificTokens(StringBuilder sb, string ilLine)
        {
            if (PotentiallyUnsafeLineRegex.IsMatch(ilLine))
            {
                if (MvidRegexp.IsMatch(ilLine))
                    sb.AppendLine(MvidRegexp.Replace(ilLine, "{00000000-0000-0000-0000-000000000000}"));
                else if (ImageBaseRegexp.IsMatch(ilLine))
                    sb.AppendLine(ImageBaseRegexp.Replace(ilLine, "0x00000000"));
                else if (PrivateImplementationDetailsGuidRegexp.IsMatch(ilLine))
                    sb.AppendLine(PrivateImplementationDetailsGuidRegexp.Replace(ilLine,
                        "{00000000-0000-0000-0000-000000000000}"));
                else if (PrivateImplementationDetailsRegexp2.IsMatch(ilLine))
                    sb.AppendLine(PrivateImplementationDetailsRegexp2.Replace(ilLine,
                        "'0000000000000000000000000000000000000000' at I_00000000"));
                else if (PrivateImplementationDetailsDataCilRegexp.IsMatch(ilLine))
                    sb.AppendLine(PrivateImplementationDetailsDataCilRegexp.Replace(ilLine, "I_00000000 = bytearray"));
                else if (ReferenceVersionNumberRegexp.IsMatch(ilLine))
                    sb.AppendLine(ReferenceVersionNumberRegexp.Replace(ilLine, "0:0:0:0"));
                else if (FooterRegexp.IsMatch(ilLine))
                    sb.AppendLine(FooterRegexp.Replace(ilLine, ""));
                else if (!ExtraDataCilRegexp.IsMatch(ilLine))
                {
                    sb.AppendLine(ilLine);
                }
            }
            else
            {
                sb.AppendLine(ilLine);
            }
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
