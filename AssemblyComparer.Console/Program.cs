namespace AssemblyComparer.Console
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (!args[1].Equals("-"))
            {
                System.Console.WriteLine("asmcomp version 0.1");
                System.Console.WriteLine(string.Format("Converting {0} to comparable text representation at {1}", args[0], args[1]));
            }
            var converter = new ComparisonConverter();
            converter.Convert(args[0], args[1]);
        }
    }
}
