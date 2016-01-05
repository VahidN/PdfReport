using PdfRpt.Calendar;

namespace PdfReportSamples.Models
{
    public class UserMonthCalendar
    {
        public int Id { set; get; }
        public string Name { set; get; }
        // Calendar's cell data type should be CalendarData
        public CalendarData MonthCalendarData { set; get; }
    }
}
