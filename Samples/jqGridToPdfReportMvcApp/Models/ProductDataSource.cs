using System;
using System.Collections.Generic;

namespace jqGridToPdfReportMvcApp.Models
{
    public static class ProductDataSource
    {
        private static readonly IList<Product> _cachedItems;
        static ProductDataSource()
        {
            _cachedItems = createProductsDataSource();
        }

        public static IList<Product> LatestProducts
        {
            get { return _cachedItems; }
        }

        private static IList<Product> createProductsDataSource()
        {
            var list = new List<Product>();
            for (var i = 0; i < 500; i++)
            {
                list.Add(new Product
                {
                    Id = i + 1,
                    Name = "Name " + (i + 1),
                    Price = i * 1000,
                    AddDate = DateTime.Now.AddDays(-i)
                });
            }
            return list;
        }
    }
}