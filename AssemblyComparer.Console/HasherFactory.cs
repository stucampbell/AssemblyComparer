using System;
using System.IO;

namespace AssemblyComparer.Console
{
    internal class HasherFactory
    {
        public static IHasher CreateHasher(string sourcePath)
        {
            if (IsCliAssembly(sourcePath))
            {
                return new CliAssemblyHasher();
            }
            return new DefaultHasher();
        }

        private static bool IsCliAssembly(string sourcePath)
        {
            var dataDictionaryRVA = new uint[16];
            var dataDictionarySize = new uint[16];

            using (Stream fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            {
                var reader = new BinaryReader(fs);

                //PE Header starts @ 0x3C (60). Its a 4 byte header.
                fs.Position = 0x3C;
                uint peHeader = reader.ReadUInt32();

                //Moving to PE Header start location...
                fs.Position = peHeader;
                reader.ReadUInt32();
                reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt16();
                reader.ReadUInt16();


                var dataDictionaryStart = Convert.ToUInt16(Convert.ToUInt16(fs.Position) + 0x60);
                fs.Position = dataDictionaryStart;
                for (int i = 0; i < 15; i++)
                {
                    dataDictionaryRVA[i] = reader.ReadUInt32();
                    dataDictionarySize[i] = reader.ReadUInt32();
                }
                return dataDictionaryRVA[14] != 0;
            }
        }
    }
}