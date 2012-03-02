using Xunit;
using AssemblyComparer.Console;

namespace AssemblyComparer.Tests
{
    public class HasherFactoryTests
    {
        [Fact]
        public void creates_default_hasher_for_win32_exe()
        {
            // arrange

            // act
            var hasher = HasherFactory.CreateHasher(@"..\..\Samples\Win32Identical.build1\win32example.exe");

            // assert
            Assert.IsAssignableFrom<DefaultHasher>(hasher);

        }

        [Fact]
        public void creates_cli_assembly_hasher_for_cli_exe()
        {
            // arrange

            // act
            var hasher = HasherFactory.CreateHasher(@"..\..\Samples\CliIdentical.build1\asmcomp.exe");

            // assert
            Assert.IsAssignableFrom<CliAssemblyHasher>(hasher);

        }
    }
}
