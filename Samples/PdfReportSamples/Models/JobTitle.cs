using System.ComponentModel;

namespace PdfReportSamples.Models
{
    public enum JobTitle
    {
        [Description("Grunt")]
        Grunt,

        [Description("Programmer")]
        Programmer,

        [Description("Analyst Programmer")]
        AnalystProgrammer,

        [Description("Project Manager")]
        ProjectManager,

        [Description("Chief Information Officer")]
        ChiefInformationOfficer,
    }
}
