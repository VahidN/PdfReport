
namespace PdfReportSamples.Models
{
    public class Task
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public int PercentCompleted { set; get; }
        public bool IsActive { set; get; }
        public User Assignee { set; get; }
    }
}
