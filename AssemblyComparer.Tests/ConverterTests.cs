using System;
using System.IO;
using System.Text;
using Xunit;
using AssemblyComparer.Console;

namespace AssemblyComparer.Tests
{
    public class ConverterTests
    {
        private TextWriter normalOutput;
        private readonly StringWriter testingConsole;
        private readonly StringBuilder testOutput;
        
        [Fact]
        public void given_source_and_target_paths_the_program_creates_the_target_file()
        {
            // arrange
            var src = @"..\..\Samples\CliIdentical.build1\asmcomp.exe";
            var target = Path.GetTempPath() + Guid.NewGuid();
            var converter = new ComparisonConverter();
            
            // act
            converter.Convert(src, target);

            // assert
            Assert.True(File.Exists(target));

        }
    }
}
