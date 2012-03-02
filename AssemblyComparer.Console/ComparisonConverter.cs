using System.IO;

namespace AssemblyComparer.Console
{
    internal class ComparisonConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            var hasher = HasherFactory.CreateHasher(sourcePath);

            File.WriteAllText(targetPath, hasher.GetHashForComparison(sourcePath));
        }
    }
}