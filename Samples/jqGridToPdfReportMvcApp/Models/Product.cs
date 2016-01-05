using System;

namespace jqGridToPdfReportMvcApp.Models
{
    public class Product
    {
        public int Id { set; get; }
        public DateTime AddDate { set; get; }
        public string Name { set; get; }
        public decimal Price { set; get; }
    }
}