using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LandRegistryParser;
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
            if (args.Length != 2)
            {
                Console.WriteLine("Please pass 2 arguments: the source PDF file, and the result CSV file");
                Console.ReadLine();
                return;
            }
            var model = Converter.Convert(args[0]);
            var result = FuncParser.Parse(model, KeyOffset, ValueOffset, DelimeterOffset).Skip(1); //skip report indo
            var models = ModelConverter.ConvertDictionaryToModels(result);
            File.WriteAllText(args[1], FormatCsv(models));
        }

        static string FormatCsv(List<Owner> owners)
        {
            var sb = new StringBuilder();
            var props = typeof(Owner).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //write a header
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                sb.Append("\"").Append(prop.Name).Append("\"");
                if (i == props.Length - 1)
                {
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(",");
                }
            }
            sb.AppendLine("");
            foreach (var owner in owners)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    var prop = props[i];
                    var value = prop.GetValue(owner);
                    sb.Append("\"").Append(value).Append("\"");
                    if (i == props.Length - 1)
                    {
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.Append(",");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
