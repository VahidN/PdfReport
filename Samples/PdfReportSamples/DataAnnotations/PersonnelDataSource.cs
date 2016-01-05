using System;
using System.Collections.Generic;
using PdfReportSamples.Models;

namespace PdfReportSamples.DataAnnotations
{
    public static class PersonnelDataSource
    {
        public static IList<Person> CreatePersonnelList()
        {
            return new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = "Edward",
                    DateOfBirth = new DateTime(1900, 1, 1),
                    DateOfDeath = new DateTime(1990, 10, 15),
                    JobTitle = JobTitle.ChiefInformationOfficer,
                    Salary = 5000
                },
                new Person
                {
                    Id = 2,
                    Name = "Margaret", 
                    DateOfBirth = new DateTime(1950, 2, 9), 
                    DateOfDeath = null,
                    JobTitle = JobTitle.AnalystProgrammer,
                    Salary = 4000
                },
                new Person
                {
                    Id = 3,
                    Name = "Grant", 
                    DateOfBirth = new DateTime(1975, 6, 13), 
                    DateOfDeath = null,
                    JobTitle = JobTitle.Programmer,
                    Salary = 3500
                }
            };
        }
    }
}
