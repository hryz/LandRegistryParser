using System.Collections.Generic;
using System.Linq;

namespace StructureConverter
{
    public class ModelConverter
    {
        private const string OwnershipKey = "Номер запису про право власності";
        public static List<Owner> ConvertDictionaryToModels(IEnumerable<List<KeyValuePair<string,string>>> sourceData)
        {
            //1. fix the Ownership key-value: [key + value : null] -> [key : value]
            var data = sourceData
                .Select(x => x
                    .Select(y => y.Key.StartsWith(OwnershipKey)
                        ? new KeyValuePair<string, string>(OwnershipKey, y.Key.Substring(OwnershipKey.Length + 2)) //skip ": "
                        : new KeyValuePair<string, string>(y.Key.Substring(0, y.Key.Length - 1), y.Value)).ToList()).ToList();

            //2. replicate the apartment data for all its owners [apart. (owner1, owner2)] -> ([arart. owner], [apart. owner])
            var multiOwner = data.Where(x => x.Any(a => a.Key == "Розмір частки" && a.Value != "1/1" && a.Value != "1")).ToList();
            var flattenedMultiOwners = new List<List<KeyValuePair<string, string>>>();
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
            return data.Select(x => new Owner(x)).ToList();
        }
    }
}
