using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace StructureConverter
{
    public class Owner
    {
        public string RealtyObjectRegNo { get; set; }
        public string RealtyObjectDescription { get; set; }
        public string Area { get; set; }
        public string Address { get; set; }
        public string OwnershipRecordRegNo { get; set; }
        public string RegistrationDate { get; set; }
        public string Registrar { get; set; }
        public string OwnershipCause { get; set; }
        public string RegistrationCause { get; set; }
        public string OwnershipType { get; set; }
        public string OwnershipPart { get; set; }
        public string OwnerName { get; set; }

        public Owner(List<KeyValuePair<string, string>> dict)
        {
            RealtyObjectRegNo = dict.FirstOrDefault(x => x.Key == "Реєстраційний номер об’єкта нерухомого майна").Value ?? String.Empty;
            RealtyObjectDescription = dict.FirstOrDefault(x => x.Key == "Об’єкт нерухомого майна").Value ?? String.Empty;
            Area = dict.FirstOrDefault(x => x.Key == "Площа").Value ?? String.Empty;
            Address = dict.FirstOrDefault(x => x.Key == "Адреса").Value ?? String.Empty;
            OwnershipRecordRegNo = dict.FirstOrDefault(x => x.Key == "Номер запису про право власності").Value ?? String.Empty;
            RegistrationDate = dict.FirstOrDefault(x => x.Key == "Дата, час державної реєстрації").Value ?? String.Empty;
            Registrar = dict.FirstOrDefault(x => x.Key == "Державний реєстратор").Value ?? String.Empty;
            OwnershipCause = dict.FirstOrDefault(x => x.Key == "Підстава виникнення права власності").Value ?? String.Empty;
            RegistrationCause = dict.FirstOrDefault(x => x.Key == "Підстава внесення запису").Value ?? String.Empty;
            OwnershipType = dict.FirstOrDefault(x => x.Key == "Форма власності").Value ?? String.Empty;
            OwnershipPart = dict.FirstOrDefault(x => x.Key == "Розмір частки").Value ?? String.Empty;
            OwnerName = dict.FirstOrDefault(x => x.Key == "Власники").Value ?? String.Empty;
            //=========================================================================================================
            OwnershipRecordRegNoInt = Int64.Parse(OwnershipRecordRegNo);
            RealtyObjectRegNoInt = Int64.Parse(RealtyObjectRegNo);
            var reg = Regex.Match(Area, @"Загальна площа \(кв\.м\): ([0-9\.]*)(, житлова площа \(кв\.м\): ([0-9\.]*))?");
            decimal totalArea, livingArea;
            TotalArea = Decimal.TryParse(reg.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out totalArea) 
                ? totalArea 
                : 0;
            LivingArea = Decimal.TryParse(reg.Groups[3].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out livingArea) 
                ? livingArea 
                : 0;
            var reg2 = Regex.Match(Address, @"(приміщення|квартира) ([0-9]*)");
            IsApartment = reg2.Groups[1].Value == "квартира";
            IsOffice = reg2.Groups[1].Value == "приміщення";
            int room, numerator, denominator;
            RoomNo = Int32.TryParse(reg2.Groups[2].Value, out room) ? room : 0;
            DateTime regDate;
            RegistrationDateTime = DateTime.TryParseExact(RegistrationDate, "dd.MM.yyyy HH:mm:ss",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out regDate)
                    ? regDate
                    : DateTime.MinValue;
            var reg3 = Regex.Match(OwnershipPart, @"([0-9]?)/?([0-9]?)");
            numerator = Int32.TryParse(reg3.Groups[1].Value, out numerator) 
                ? numerator 
                : 1;
            denominator = Int32.TryParse(reg3.Groups[2].Value, out denominator) 
                ? denominator 
                : 1;
            Part = (decimal)numerator / denominator;
        }

        public long OwnershipRecordRegNoInt { get; set; }
        public long RealtyObjectRegNoInt { get; set; }
        public decimal TotalArea { get; set; }
        public decimal LivingArea { get; set; }
        public bool IsApartment { get; set; }
        public bool IsOffice { get; set; }
        public int RoomNo { get; set; }
        public DateTime RegistrationDateTime { get; set; }
        public decimal Part { get; set; }
    }
}
