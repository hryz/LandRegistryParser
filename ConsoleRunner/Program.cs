using System.Collections.Generic;
using System.IO;
using System.Linq;
using LandRegistryParser;
using Newtonsoft.Json;

namespace ConsoleRunner
{
    class Program
    {
        private const decimal KeyOffset = 59.53M;
        private const decimal ValueOffset = 201.26M;
        private const decimal DelimeterOffset = 168.5M;

        private const string OwnershipKey = "Номер запису про право власності";
        static void Main(string[] args)
        {
            var model = Converter.Convert("..\\..\\src.pdf");
            var result = FuncParser.Parse(model, KeyOffset, ValueOffset, DelimeterOffset).Skip(1); //skip report indo

            //1. fix the Ownership key-value: [key + value : null] -> [key : value]
            var data = result
                .Select(x => x
                    .Select(y => y.Key.StartsWith(OwnershipKey)
                        ? new KeyValuePair<string, string>(OwnershipKey, y.Key.Substring(OwnershipKey.Length + 2)) //skip ": "
                        : new KeyValuePair<string, string>(y.Key.Substring(0, y.Key.Length -1) ,y.Value)).ToList()).ToList();

            //2. replicate the apartment data for all its owners [apart. (owner1, owner2)] -> ([arart. owner], [apart. owner])
            var multiOwner = data.Where(x => x.Any(a => a.Key == "Розмір частки" && a.Value != "1/1" && a.Value != "1")).ToList();
            var flattenedMultiOwners = new List<List<KeyValuePair<string,string>>>();
            foreach (var ap in multiOwner)
            {
                var apFields = ap.TakeWhile(x => x.Key != OwnershipKey).ToList();
                var owners = ap
                    .Where(x => x.Key == OwnershipKey)
                    .Select(x => x.Value)
                    .Select(z => apFields.Union(ap
                        .SkipWhile(x => !(x.Key == OwnershipKey && x.Value == z))
                        .TakeWhile(x => !(x.Key == OwnershipKey && x.Value != z))).ToList())
                    .ToList();
                flattenedMultiOwners.AddRange(owners);
            }
            data.RemoveAll(r => multiOwner.Contains(r));
            data.AddRange(flattenedMultiOwners);
            //3. project the dictionaries onto the models
            var ownerObjects = data.Select(x => new Owner(x)).ToList();
            File.WriteAllText("..\\..\\owners.json", JsonConvert.SerializeObject(ownerObjects,Formatting.Indented));
        }
    }
}
