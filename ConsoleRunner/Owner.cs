using System.Collections.Generic;
using System.Linq;

namespace ConsoleRunner
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
            RealtyObjectRegNo = dict.FirstOrDefault(x => x.Key == "Реєстраційний номер об’єкта нерухомого майна").Value;
            RealtyObjectDescription = dict.FirstOrDefault(x => x.Key == "Об’єкт нерухомого майна").Value;
            Area = dict.FirstOrDefault(x => x.Key == "Площа").Value;
            Address = dict.FirstOrDefault(x => x.Key == "Адреса").Value;
            OwnershipRecordRegNo = dict.FirstOrDefault(x => x.Key == "Номер запису про право власності").Value;
            RegistrationDate = dict.FirstOrDefault(x => x.Key == "Дата, час державної реєстрації").Value;
            Registrar = dict.FirstOrDefault(x => x.Key == "Державний реєстратор").Value;
            OwnershipCause = dict.FirstOrDefault(x => x.Key == "Підстава виникнення права власності").Value;
            RegistrationCause = dict.FirstOrDefault(x => x.Key == "Підстава внесення запису").Value;
            OwnershipType = dict.FirstOrDefault(x => x.Key == "Форма власності").Value;
            OwnershipPart = dict.FirstOrDefault(x => x.Key == "Розмір частки").Value;
            OwnerName = dict.FirstOrDefault(x => x.Key == "Власники").Value;
        }
    }
}
