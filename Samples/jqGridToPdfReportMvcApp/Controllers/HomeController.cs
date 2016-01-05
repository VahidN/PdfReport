using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using jqGridToPdfReportMvcApp.Extensions;
using jqGridToPdfReportMvcApp.Models;
using jqGridToPdfReportMvcApp.PdfReports;

namespace jqGridToPdfReportMvcApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProducts(JqGridRequest request)
        {
            var list = ProductDataSource.LatestProducts;

            var pageIndex = request.page - 1;
            var pageSize = request.rows;
            var totalRecords = list.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);

            var productsQuery = list.AsQueryable();

            productsQuery = new JqGridSearch().ApplyFilter(productsQuery, request, this.Request.Form);
            productsQuery = productsQuery.OrderBy(request.sidx + " " + request.sord);

            if (string.IsNullOrWhiteSpace(request.oper))
            {
                productsQuery = productsQuery
                                    .Skip(pageIndex * pageSize)
                                    .Take(pageSize);
            }
            else if (request.oper == "excel")
            {
                productsQuery = productsQuery
                                    .Skip(pageIndex * pageSize);
            }

            var productsList = productsQuery.ToList();

            if (!string.IsNullOrWhiteSpace(request.oper) && request.oper == "excel")
            {
                new ProductsPdfReport().CreatePdfReport(productsList);
            }

            var productsData = new JqGridData
            {
                Total = totalPages,
                Page = request.page,
                Records = totalRecords,
                Rows = (productsList.Select(product => new JqGridRowData
                {
                    Id = product.Id,
                    RowCells = new List<string>
                    {
                        product.Id.ToString(CultureInfo.InvariantCulture),
                        product.Name,
                        product.AddDate.ToString("yyyy-MM-dd"),
                        product.Price.ToString(CultureInfo.InvariantCulture)
                    }
                })).ToArray()
            };

            return Json(productsData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductNames(string term)
        {
            var list = ProductDataSource.LatestProducts
                .Where(x => x.Name.StartsWith(term))
                .Select(x => x.Name)
                .Take(10)
                .ToArray();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}