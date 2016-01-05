using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PdfReportSamples.Models;
using PdfRpt.Aggregates.Numbers;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.DataAnnotations;

namespace PdfReportSamples.DataAnnotations
{
    public class Person
    {
        [IsVisible(false)]
        public int Id { get; set; }

        [DisplayName("User name")]
        //Note: If you don't specify the ColumnItemsTemplate, a new TextBlockField() will be used automatically.
        [ColumnItemsTemplate(typeof(TextBlockField))]
        public string Name { get; set; }

        [DisplayName("Job title")]
        public JobTitle JobTitle { set; get; }

        [DisplayName("Date of birth")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfBirth { get; set; }

        [DisplayName("Date of death")]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateOfDeath { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        [CustomAggregateFunction(typeof(Sum))]
        public int Salary { get; set; }

        [IsCalculatedField(true)]
        [DisplayName("Calculated Field")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        [AggregateFunction(AggregateFunction.Sum)]
        public string CalculatedField { get; set; }
        
        [CalculatedFieldFormula("CalculatedField")]
        public static Func<IList<CellData>, object> CalculatedFieldFormula =
                                                    list =>
                                                    {
                                                        if (list == null) return string.Empty;
                                                        var salary = (int)list.GetValueOf<Person>(x => x.Salary);
                                                        return salary * 0.8;
                                                    };//Note: It's a static field, not a property.

        //and for .... [IncludeInGrouping(true)]
        /*[IncludedGroupFieldEqualityComparer("CalculatedField")]
        public static Func<object, object, bool> IncludedGroupFieldEqualityComparer =
                                                    (val1, val2) =>
                                                    {
                                                        if (val1 == null && val2 == null) return true;
                                                        if (val1 == null || val2 == null) return false;
                                                        return val1.ToString() == val2.ToString();
                                                    };//Note: It's a static field, not a property.*/
    }
}
