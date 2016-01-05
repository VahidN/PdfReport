using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PdfReportSamples.Models;
using PdfRpt.DataSources;

namespace PdfReportSamples.ExtraHeadingCells
{
    public static class DataGenerator
    {
        public static IEnumerable ContactsList()
        {
            return createContacts().flattenContacts()
                        .Pivot(
                            x =>
                               new
                               {
                                   x.Id,
                                   x.FirstName,
                                   x.LastName
                               },
                            x1 => x1.PhoneType,
                            persons => string.Concat("(", persons.First().AreaCode, ") ", persons.First().PhoneNumber),
                            x3 => new { Count = x3.Count() });
        }

        private static IEnumerable<FlatContact> flattenContacts(this IList<Person> source)
        {
            foreach (var person in source)
                foreach (var phone in person.Phones)
                    yield return new FlatContact
                    {
                        Id = person.Id,
                        FirstName = person.Name,
                        LastName = person.LastName,
                        PhoneType = phone.PhoneType,
                        PhoneNumber = phone.PhoneNumber,
                        AreaCode = phone.AreaCode
                    };
        }

        private static IList<Person> createContacts()
        {
            return new List<Person>
            {
                new Person 
                {
                    Id = 1, 
                    Name = "John", 
                    LastName = "Doe",
                    Phones = new List<Phone> 
                                 {
                                     new Phone(PhoneType.Home,   "305", "555-1111"),
                                     new Phone(PhoneType.Office, "305", "555-2222"),
                                     new Phone(PhoneType.Cell,   "305", "555-3333")
                                 }
                },
                new Person 
                {
                    Id = 2, 
                    Name = "Jane",
                    LastName = "Doe", 
                    Phones = new List<Phone> 
                                 {
                                     new Phone(PhoneType.Home,   "305", "555-1111"),
                                     new Phone(PhoneType.Office, "305", "555-4444"),
                                     new Phone(PhoneType.Cell,   "305", "555-5555") 
                                 }
                },
                new Person 
                {
                    Id = 3, 
                    Name = "Jerome", 
                    LastName = "Doe", 
                    Phones = new List<Phone> 
                                 { 
                                     new Phone(PhoneType.Home,   "305", "555-6666"),
                                     new Phone(PhoneType.Office, "305", "555-2222"),
                                     new Phone(PhoneType.Cell,   "305", "555-7777") 
                                 }
                },
                new Person 
                {
                    Id = 4, 
                    Name = "Joel", 
                    LastName = "Smith", 
                    Phones = new List<Phone> 
                                 {
                                     new Phone(PhoneType.Fax,    "305", "555-6666"),
                                     new Phone(PhoneType.Cell,   "305", "555-7777")
                                 }
                } 
            };
        }
    }
}
