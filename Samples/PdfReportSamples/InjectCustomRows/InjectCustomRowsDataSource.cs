using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PdfReportSamples.Models;

namespace PdfReportSamples.InjectCustomRows
{
    public static class InjectCustomRowsDataSource
    {
        public static IEnumerable MyTransactions()
        {
            var list = createTransactions();
            return list.Select(t => new
            {
                Id = t.Id,
                Date = t.Date,
                Description = t.Description,
                Income = t.Type == TransactionType.Income ? t.SalePrice : 0,
                Payment = t.Type == TransactionType.Payment ? t.SalePrice : 0,
                Residue = t.Type == TransactionType.Income ? (int)t.SalePrice : (int)-t.SalePrice
            });
        }

        private static IList<Transaction> createTransactions()
        {
            return new[]
            {
                new Transaction
                {
                     Id = 1,
                     Date = DateTime.Now.AddDays(-10),
                     Description = "Desc 1",
                     SalePrice = 10,
                     Type = TransactionType.Income
                },
                new Transaction
                {
                     Id = 2,
                     Date = DateTime.Now.AddDays(-9),
                     Description = "Desc 2",
                     SalePrice = 2,
                     Type = TransactionType.Payment
                },
                new Transaction
                {
                     Id = 3,
                     Date = DateTime.Now.AddDays(-8),
                     Description = "Desc 3",
                     SalePrice = 5,
                     Type = TransactionType.Payment
                },
                new Transaction
                {
                     Id = 4,
                     Date = DateTime.Now.AddDays(-7),
                     Description = "Desc 4",
                     SalePrice = 7,
                     Type = TransactionType.Income
                },
                new Transaction
                {
                     Id = 5,
                     Date = DateTime.Now.AddDays(-5),
                     Description = "Desc 5",
                     SalePrice = 6,
                     Type = TransactionType.Payment
                }                 
            };
        }
    }
}
