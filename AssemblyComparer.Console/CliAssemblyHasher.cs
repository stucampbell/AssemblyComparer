using Mono.Cecil;

namespace AssemblyComparer.Console
{
    internal class CliAssemblyHasher : IHasher
    {
        public string GetHashForComparison(string sourcePath)
        {
            var module = ModuleDefinition.ReadModule(sourcePath);
            return module.GetBuildIndependentHash();
        }
    }
}