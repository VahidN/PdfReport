using System;
using System.Collections.Generic;

namespace PdfReportSamples.LongTextInCell
{
    [Serializable]
    public class ApprovalReport
    {
        public string ReportTitle { get { return "Лист согласования"; } }

        public string DocumentTitle { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string WorkflowInitiator { get; set; }

        public string WorkflowInitiatorUrl { get; set; }

        public IEnumerable<Approval> Approvals { get; set; }

        public Guid InstanceId { get; set; }

        [Serializable]
        public class Approval
        {
            public string Url { get; set; }

            public string Number { get; set; }

            public string Approver { get; set; }

            public string ApproverUrl { get; set; }

            public DateTime? ApprovalDate { get; set; }

            public string Result { get; set; }

            public string Commentary { get; set; }

            public string Department { get; set; }

            public string Position { get; set; }
        }
    }
}