using System;
using System.Collections.Generic;
using System.Linq;
using PdfReportSamples.Models;
using PdfRpt.Calendar;

namespace PdfReportSamples.PersianMonthCalendar
{
    public static class PersianMonthCalendarDataSource
    {
        public static IList<UserMonthCalendar> CreateDataSource()
        {
            var usersWorkedHours = createUsersWorkedHours();
            // Mapping a list of normal Users WorkedHours to a list of Users + CalendarData
            return usersWorkedHours
                        .GroupBy(x => new
                        {
                            Id = x.Id,
                            Name = x.Name
                        })
                        .Select(
                                 x => new UserMonthCalendar
                                 {
                                     Id = x.Key.Id,
                                     Name = x.Key.Name,
                                     // Calendar's cell data type should be PdfRpt.Calendar.CalendarData
                                     MonthCalendarData = new CalendarData
                                     {
                                         Year = x.First().Year,
                                         Month = x.First().Month,
                                         MonthDaysInfo = x.ToList().Select(y => new DayInfo
                                         {
                                             Description = y.Description,
                                             ShowDescriptionInFooter = false,
                                             DayNumber = y.DayNumber
                                         }).ToList()
                                     }
                                 }).ToList();
        }

        private static List<UserWorkedHours> createUsersWorkedHours()
        {
            var usersWorkedHours = new List<UserWorkedHours>();
            for (int i = 1; i < 11; i++)
            {
                for (int j = 1; j < 28; j++)
                {
                    usersWorkedHours.Add(new UserWorkedHours
                    {
                        Id = i,
                        Name = "كارمند " + i,
                        Year = 1391, // سال و ماه بر اساس نوع تقويم انتخابي مشخص مي‌شود
                        Month = i,
                        DayNumber = j,
                        Description = i % 2 == 0 ? "05:00" : "08:00"
                    });
                }
            }
            return usersWorkedHours;
        }
    }
}