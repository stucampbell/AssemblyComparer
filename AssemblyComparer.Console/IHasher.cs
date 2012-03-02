namespace AssemblyComparer.Console
{
    internal interface IHasher
    {
        string GetHashForComparison(string sourcePath);
    }
}