using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using jqGridToPdfReportMvcApp.Models;
using Newtonsoft.Json;

namespace jqGridToPdfReportMvcApp.Extensions
{
    // this class is based on the default values of `prmNames`

    //single field search
    //_search=true&nd=1403935889318&rows=10&page=1&sidx=Id&sord=asc&searchField=Id&searchString=4444&searchOper=eq&filters=

    //multi-field search
    //_search=true&nd=1403935941367&rows=10&page=1&sidx=Id&sord=asc&filters=%7B%22groupOp%22%3A%22AND%22%2C%22rules%22%3A%5B%7B%22field%22%3A%22Id%22%2C%22op%22%3A%22eq%22%2C%22data%22%3A%2244%22%7D%2C%7B%22field%22%3A%22SupplierID%22%2C%22op%22%3A%22eq%22%2C%22data%22%3A%221%22%7D%5D%7D&searchField=&searchString=&searchOper=
    // filters -> {"groupOp":"AND","rules":[{"field":"All","op":"cn","data":"fffff"},{"field":"Price","op":"bn","data":"ffff"}]}

    //toolbar search
    //_search=true&nd=1403935593036&rows=10&page=1&sidx=Id&sord=asc&Id=2&Name=333&SupplierID=1&CategoryID=1&Price=44

    public class JqGridSearch
    {
        private static readonly Dictionary<string, string> _whereOperation =
                     new Dictionary<string, string>  
	                 {  
                        {"in" , " {0} = @{1} "},//is in
                        {"eq" , " {0} = @{1} "},
                        {"ni" , " {0} != @{1} "},//is not in
                        {"ne" , " {0} != @{1} "},
                        {"lt" , " {0} < @{1} "},
                        {"le" , " {0} <= @{1} "},
                        {"gt" , " {0} > @{1} "},
                        {"ge" , " {0} >= @{1} "},
                        {"bw" , " {0}.StartsWith(@{1}) "},//begins with
                        {"bn" , " !{0}.StartsWith(@{1}) "},//does not begin with
                        {"ew" , " {0}.EndsWith(@{1}) "},//ends with
                        {"en" , " !{0}.EndsWith(@{1}) "},//does not end with
                        {"cn" , " {0}.Contains(@{1}) "},//contains
                        {"nc" , " !{0}.Contains(@{1}) "}//does not contain
	                 };

        /// <summary>
        /// هر اپراتوري را به هر نوع داده‌اي نمي‌توان اعمال كرد
        /// </summary>
        private static readonly Dictionary<string, string> _validOperators =
                     new Dictionary<string, string>  
	                 {  
	                     { "Object"   ,  "" },   
	                     { "Boolean"  ,  "eq:ne:" },   
	                     { "Char"     ,  "" },   
	                     { "String"   ,  "eq:ne:lt:le:gt:ge:bw:bn:cn:nc:" },   
	                     { "SByte"    ,  "" },   
	                     { "Byte"     ,  "eq:ne:lt:le:gt:ge:" },  
	                     { "Int16"    ,  "eq:ne:lt:le:gt:ge:" },   
	                     { "UInt16"   ,  "" },   
	                     { "Int32"    ,  "eq:ne:lt:le:gt:ge:" },   
	                     { "UInt32"   ,  "" },   
	                     { "Int64"    ,  "eq:ne:lt:le:gt:ge:" },   
	                     { "UInt64"   ,  "" },   
	                     { "Decimal"  ,  "eq:ne:lt:le:gt:ge:" },   
	                     { "Single"   ,  "eq:ne:lt:le:gt:ge:" },   
	                     { "Double"   ,  "eq:ne:lt:le:gt:ge:" },   
	                     { "DateTime" ,  "eq:ne:lt:le:gt:ge:" },   
	                     { "TimeSpan" ,  "" },   
	                     { "Guid"     ,  "" }  
	                 };

        private int _parameterIndex;

        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, JqGridRequest request, NameValueCollection form)
        {
            if (!request._search)
                return query;

            if (!string.IsNullOrWhiteSpace(request.searchString) &&
                !string.IsNullOrWhiteSpace(request.searchOper) &&
                !string.IsNullOrWhiteSpace(request.searchField))
            {
                return manageSingleFieldSearch(query, request.searchField, request.searchString, request.searchOper);
            }

            return !string.IsNullOrWhiteSpace(request.filters) ?
                    manageMultiFieldSearch(query, request.filters) : manageToolbarSearch(query, form);
        }

        private Tuple<string, object> getPredicate<T>(string searchField, string searchOper, string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
                return null;

            var type = typeof(T).FindFieldType(searchField);
            if (type == null)
                throw new InvalidOperationException(searchField + " is not defined.");

            if (!_validOperators[type.Name].Contains(searchOper + ":"))
            {
                // اين اپراتور روي نوع داده‌اي جاري كار نمي‌كند  
                return null;
            }

            if (type == typeof (decimal))
            {
                decimal value;
                if (decimal.TryParse(searchValue, NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out value))
                {
                    return new Tuple<string, object>(getSearchOperator(searchOper, searchField, type), value);
                }
            }

            if (type == typeof (DateTime))
            {
                return new Tuple<string, object>(getSearchOperator(searchOper, searchField, type),
                                                DateTime.Parse(searchValue));
            }

            var resultValue = Convert.ChangeType(searchValue, type);
            return new Tuple<string, object>(getSearchOperator(searchOper, searchField, type), resultValue);
        }

        private string getSearchOperator(string ruleSearchOperator, string searchField, Type type)
        {
            string whereOperation;
            if (!_whereOperation.TryGetValue(ruleSearchOperator, out whereOperation))
            {
                throw new NotSupportedException(string.Format("{0} is not supported.", ruleSearchOperator));
            }

            if (type == typeof(DateTime))
            {
                switch (ruleSearchOperator)
                {
                    case "eq":
                        whereOperation = " {0}.Date = @{1} ";
                        break;
                    case "ne":
                        whereOperation = " {0}.Date != @{1} ";
                        break;
                }
            }

            var searchOperator = string.Format(whereOperation, searchField, _parameterIndex);

            _parameterIndex++;
            return searchOperator;
        }

        private IQueryable<T> manageMultiFieldSearch<T>(IQueryable<T> query, string filters)
        {
            var filtersArray = JsonConvert.DeserializeObject<SearchFilter>(filters);
            var groupOperator = filtersArray.groupOp;
            if (filtersArray.rules == null)
                return query;

            var valuesList = new List<object>();
            var filterExpression = String.Empty;
            foreach (var rule in filtersArray.rules)
            {
                var predicate = getPredicate<T>(rule.field, rule.op, rule.data);
                if (predicate == null)
                    continue;

                valuesList.Add(predicate.Item2);
                filterExpression = filterExpression + predicate.Item1 + " " + groupOperator + " ";
            }

            if (string.IsNullOrWhiteSpace(filterExpression))
                return query;

            filterExpression = filterExpression.Remove(filterExpression.Length - groupOperator.Length - 2);
            query = query.Where(filterExpression, valuesList.ToArray());
            return query;
        }

        private IQueryable<T> manageSingleFieldSearch<T>(IQueryable<T> query, string searchField, string searchString,
            string searchOper)
        {
            var predicate = getPredicate<T>(searchField, searchOper, searchString);
            if (predicate != null)
                query = query.Where(predicate.Item1, new[] { predicate.Item2 });
            return query;
        }

        private IQueryable<T> manageToolbarSearch<T>(IQueryable<T> query, NameValueCollection form)
        {
            var filterExpression = String.Empty;
            var valuesList = new List<object>();
            foreach (var postDataKey in form.AllKeys)
            {
                if (!postDataKey.Equals("nd") && !postDataKey.Equals("sidx")
                    && !postDataKey.Equals("sord") && !postDataKey.Equals("page")
                    && !postDataKey.Equals("rows") && !postDataKey.Equals("_search")
                    && form[postDataKey] != null)
                {
                    var predicate = getPredicate<T>(postDataKey, "eq", form[postDataKey]);
                    if (predicate == null)
                        continue;

                    valuesList.Add(predicate.Item2);
                    filterExpression = filterExpression + predicate.Item1 + " And ";
                }
            }

            if (string.IsNullOrWhiteSpace(filterExpression))
                return query;

            filterExpression = filterExpression.Remove(filterExpression.Length - 5);
            query = query.Where(filterExpression, valuesList.ToArray());
            return query;
        }
    }
}