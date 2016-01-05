using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PdfReportSamples.Models;
using PdfRpt.DataSources;

namespace PdfReportSamples.WorkedHours
{
    public class PunchOutTimePivot
    {
        int _lastId;
        int _i;
        int _idx;
        private string getHeader(int id)
        {
            if (_lastId != id)
            {
                _i = 0;
                _lastId = id;
                _idx = 0;
            }

            if (_i++ % 2 == 0)
            {
                return "In " + (_i - _idx); //in
            }
            _idx++;
            return "Out " + (_i - _idx); //out
        }

        private string calculateWorkedHours(IEnumerable<DateTime> hoursList)
        {
            if (hoursList == null || !hoursList.Any()) return "00:00";
            if (hoursList.Count() % 2 != 0) return "00:00"; //it's not balanced

            int min = 0;
            int i = 0;
            foreach (var item in hoursList)
            {
                int sign = 1;
                if (i % 2 == 0)
                {
                    sign *= -1;
                }

                min += sign * (item.Hour * 60);
                min += sign * item.Minute;

                i++;
            }

            int hours = min / 60;
            int minutes = min - (hours * 60);

            return hours.ToString("00") + ":" + minutes.ToString("00");
        }

        string getCellValue(PunchOutTimeRecord record)
        {
            return record.LogTime.Hour.ToString("00") + ":" + record.LogTime.Minute.ToString("00");
        }

        public IEnumerable GetLogTimesPivotList()
        {
            var list = PunchOutTimeSampleDataSource.GetLogTimesList()
                                                   .OrderBy(x => x.LogTime)
                                                   .ThenBy(x => x.Id);
            return list.Pivot(x =>
                               new
                               {
                                   Id = x.Id,
                                   Name = x.EmployeeName,
                                   Date = x.LogTime.Year + "/" + x.LogTime.Month + "/" + x.LogTime.Day
                               },
                              x1 => getHeader(x1.Id),
                              x2 => getCellValue(x2.First()),
                              x3 => new
                              {
                                  WorkedHours = calculateWorkedHours(x3.Select(x => x.LogTime))
                              });
        }
    }
}
