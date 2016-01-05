using System;
using System.Collections.Generic;
using PdfReportSamples.Models;

namespace PdfReportSamples.NestedProperties
{
    public static class SessionsDataSource
    {
        public static IList<WeekClassSessions> CreateSessions()
        {
            var dataSource = new List<WeekClassSessions>();
            for (int i = 0; i < 10; i++)
            {
                for (int w = 0; w < 17; w++)
                {
                    var row = new WeekClassSessions
                    {
                        ClassName = "Class " + i,
                        IsSelected = w % 2 == 0,
                        WeekNumber = w + 1,
                        WeekTitle = "Week " + (w + 1),
                        WD0 = createCell(w + 1, 0),
                        WD1 = createCell(w + 1, 1),
                        WD2 = createCell(w + 1, 2),
                        WD3 = createCell(w + 1, 3),
                        WD4 = createCell(w + 1, 4),
                        WD5 = createCell(w + 1, 5),
                        WD6 = createCell(w + 1, 6)
                    };
                    dataSource.Add(row);
                }
            }
            return dataSource;
        }

        private static ClassSession createCell(int weekNumber, int day)
        {
            int minuteSum = new Random().Next(45, 120);
            return new ClassSession
            {
                DayNumber = day,
                HasSession = true,
                Percent = (minuteSum * 100) / 720,
                Date = DateTime.Now.AddDays(day),
                WeekNumber = weekNumber + 1
            };
        }
    }
}