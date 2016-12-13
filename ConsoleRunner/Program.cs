using LandRegistryParser;

namespace ConsoleRunner
{
    class Program
    {
        private const decimal KeyOffset = 59.53M;
        private const decimal ValueOffset = 201.26M;
        private const decimal DelimeterOffset = 168.5M;

        static void Main(string[] args)
        {
            var model = Converter.Convert("..\\..\\src.pdf");
            var result = FuncParser.Parse(model, KeyOffset, ValueOffset, DelimeterOffset);
        }
    }
}
