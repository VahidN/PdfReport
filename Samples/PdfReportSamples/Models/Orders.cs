using System;
using System.Collections.Generic;

namespace PdfReportSamples.Models
{
    public class Orders
    {
        public int Id { set; get; }
        public DateTime Date { set; get; }
        public Person Customer { set; get; }
        public IList<Order> OrdersList { set; get; }
    }
}
