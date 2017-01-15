using System.Collections.Generic;
using System.IO;
using System.Linq;
using LandRegistryParser;
using Newtonsoft.Json;
using StructureConverter;

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
            var result = FuncParser.Parse(model, KeyOffset, ValueOffset, DelimeterOffset).Skip(1); //skip report indo
            var models = ModelConverter.ConvertDictionaryToModels(result);
            File.WriteAllText("..\\..\\owners.json", JsonConvert.SerializeObject(models, Formatting.Indented));
        }
    }
}
