using Xunit;
using AssemblyComparer.Console;

namespace AssemblyComparer.Tests
{
    public class DefaultHasherTests
    {

        [Fact(Skip = "Visual C++ seems to change the output even though the source is identical.")]
        public void two_independent_builds_of_the_same_source_yield_the_same_independent_build_hash()
        {
            // arrange
            var hasher = new DefaultHasher();
            const string source1 = @"..\..\Samples\Win32Identical.build1\win32example.exe";
            const string source2 = @"..\..\Samples\Win32Identical.build2\win32example.exe";

            // act
            var hash1 = hasher.GetHashForComparison(source1);
            var hash2 = hasher.GetHashForComparison(source2);

            // assert
            Assert.Equal(hash1, hash2);
        }

       [Fact(Skip = "Visual C++ seems to change the output even though the source is identical.")]
        public void two_builds_of_slightly_differing_source_code_yield_different_independent_build_hashes()
        {
            // arrange
            var hasher = new DefaultHasher();
            const string source1 = @"..\..\Samples\Win32Identical.build2\win32example.exe";
            const string source2 = @"..\..\Samples\Win32Different.build1\win32example.exe";

            // act
            var hash1 = hasher.GetHashForComparison(source1);
            var hash2 = hasher.GetHashForComparison(source2);

            // assert
            Assert.NotEqual(hash1, hash2);
        }
    }
}
