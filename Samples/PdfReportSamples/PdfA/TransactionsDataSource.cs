using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PdfReportSamples.Models;
using PdfRpt.DataSources;

namespace PdfReportSamples.PdfA
{
    public static class TransactionsDataSource
    {
        public static IEnumerable PivotTransactionsList()
        {
            return createTransactionsList()
                      .Pivot(
                            x => new
                            {
                                SalesPerson = x.SalesPerson
                            },
                            x1 => x1.Product + " SalePrice",
                            x2 => x2.Sum(x4 => x4.SalePrice),
                            x8 => x8.Product + " Count",
                            x7 => x7.Count(),
                            x3 => new
                            {
                                Count = x3.Count(),
                                SalePrice = x3.Sum(x6 => x6.SalePrice)
                            });
        }

        private static IList<Transaction> createTransactionsList()
        {
            return new List<Transaction>
            {
                  new Transaction(new DateTime(2011, 11, 28), "Chris", "Corolla", 4000F),
                  new Transaction(new DateTime(2011, 11, 29), "Brian", "Prius", 2000F),
                  new Transaction(new DateTime(2011, 11, 30), "Chris", "Camry", 5000F),
                  new Transaction(new DateTime(2011, 11, 30), "Jason", "Corolla", 1000F),
                  new Transaction(new DateTime(2011, 12, 1),  "Brian", "Camry",  4000F),
                  new Transaction(new DateTime(2011, 12, 1),  "Chris", "Camry", 2000F),
                  new Transaction(new DateTime(2011, 12, 2),  "Chris", "Corolla", 2500F),
                  new Transaction(new DateTime(2011, 12, 3),  "Jason", "Camry", 1100F),
                  new Transaction(new DateTime(2011, 12, 4),  "Brian", "Corolla", 1200F),
                  new Transaction(new DateTime(2011, 12, 4),  "Jason", "Prius", 1700F),
                  new Transaction(new DateTime(2011, 12, 5),  "Brian", "Corolla", 900F),
                  new Transaction(new DateTime(2011, 12, 6),  "Jason", "Prius", 1300F),
                  new Transaction(new DateTime(2011, 12, 7),  "Chris", "Prius", 2000F)
            };
        }
    }
}
