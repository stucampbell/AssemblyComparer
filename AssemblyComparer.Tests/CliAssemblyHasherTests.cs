using Xunit;
using AssemblyComparer.Console;

namespace AssemblyComparer.Tests
{
    public class CliAssemblyHasherTests
    {
        [Fact]
        public void two_independent_builds_of_the_same_source_yield_the_same_independent_build_hash()
        {
            // arrange
            var hasher = new CliAssemblyHasher();
            const string source1 = @"..\..\Samples\CliIdentical.build1\asmcomp.exe";
            const string source2 = @"..\..\Samples\CliIdentical.build2\asmcomp.exe";

            // act
            var hash1 = hasher.GetHashForComparison(source1);
            var hash2 = hasher.GetHashForComparison(source2);

            // assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void two_builds_of_slightly_differing_source_code_yield_different_independent_build_hashes()
        {
            // arrange
            var hasher = new CliAssemblyHasher();
            const string source1 = @"..\..\Samples\CliIdentical.build2\asmcomp.exe";
            const string source2 = @"..\..\Samples\CliDifferent.build1\asmcomp.exe";

            // act
            var hash1 = hasher.GetHashForComparison(source1);
            var hash2 = hasher.GetHashForComparison(source2);

            // assert
            Assert.NotEqual(hash1, hash2);
        }
    }
}
