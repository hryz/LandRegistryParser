using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace StructureConverter
{
    public class Owner
    {
        public string RealtyObjectDescription { get; }
        public string Area { get; }
        public string Address { get; }
        public string OwnershipType { get; }
        public string OwnerName { get; }
        public long OwnershipRecordNo { get; }
        public long RealtyObjectNo { get; }
        public decimal TotalArea { get; }
        public decimal LivingArea { get; }
        public bool IsApartment { get; }
        public bool IsOffice { get; }
        public string RoomNo { get; }
        public decimal Part { get; }

        public Owner(List<KeyValuePair<string, string>> dict)
        {
            var realtyObjectRegNo = GetValue(dict, "Реєстраційний номер об’єкта нерухомого майна");
            RealtyObjectDescription = GetValue(dict, "Об’єкт нерухомого майна");
            Area = GetValue(dict, "Площа");
            Address = GetValue(dict, "Адреса");
            var ownershipRecordRegNo = GetValue(dict, "Номер запису про право власності");
            OwnershipType = GetValue(dict, "Форма власності");
            var ownershipPart = GetValue(dict, "Розмір частки");
            OwnerName = GetValue(dict, "Власники");

            //Process parsed data
            OwnershipRecordNo = TryParseLong(ownershipRecordRegNo, out var ownRegNo) ? ownRegNo : 0L;
            RealtyObjectNo = TryParseLong(realtyObjectRegNo, out var objRegNo) ? objRegNo : 0L;

            var areaRegExp = Regex.Match(Area, @"Загальна площа \(кв\.м\): ([0-9\.]*)(, житлова площа \(кв\.м\): ([0-9\.]*))?.*");
            if (areaRegExp.Groups.Count >= 4)
            {
                TotalArea = TryParseDecimal(areaRegExp.Groups[1].Value, out var totalArea) ? totalArea : 0;
                LivingArea = TryParseDecimal(areaRegExp.Groups[3].Value, out var livingArea) ? livingArea : 0;
            }

            var partRegExp = Regex.Match(Address, @"(приміщення|квартира) ([0-9\.]*)");
            if (partRegExp.Groups.Count >= 2)
            {
                IsApartment = partRegExp.Groups[1].Value == "квартира";
                IsOffice = partRegExp.Groups[1].Value == "приміщення";
            }

            if (partRegExp.Groups.Count >= 3)
            {
                int numerator, denominator;
                RoomNo = partRegExp.Groups[2].Value;

                var reg3 = Regex.Match(ownershipPart, @"([0-9]?)/?([0-9]?)");
                numerator = int.TryParse(reg3.Groups[1].Value, out numerator) ? numerator : 1;
                denominator = int.TryParse(reg3.Groups[2].Value, out denominator) ? denominator : 1;
                Part = (decimal)numerator / denominator;
            }
        }

        private static string GetValue(IEnumerable<KeyValuePair<string, string>> dict, string key) =>
            dict.FirstOrDefault(x => x.Key == key).Value ?? string.Empty;

        private static bool TryParseDecimal(string src, out decimal result) =>
            decimal.TryParse(src, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

        private static bool TryParseLong(string src, out long result) =>
            long.TryParse(src, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }
}
