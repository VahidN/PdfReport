using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PdfRpt.Core.Contracts;

namespace PdfReportSamples.StackedProperties
{
    public class CustomSum : IAggregateFunction
    {
        const string Pattern = @"<td\b[^>]*?>(?<V>[\s\S]*?)</\s*td>";
        private static readonly Regex _valueFormatMatch = new Regex(Pattern, RegexOptions.Compiled);

        float _groupSum;
        float _overallSum;

        /// <summary>
        /// Fires before rendering of this cell.
        /// Now you have time to manipulate the received object and apply your custom formatting function.
        /// It can be null.
        /// </summary>
        public Func<object, string> DisplayFormatFormula { set; get; }

        /// <summary>
        /// Returns current groups' aggregate value.
        /// </summary>
        public object GroupValue
        {
            get { return _groupSum; }
        }

        /// <summary>
        /// Returns current row's aggregate value without considering the presence of the groups.
        /// </summary>
        public object OverallValue
        {
            get { return _overallSum; }
        }

        /// <summary>
        /// Fires after adding a cell to the main table.
        /// </summary>
        /// <param name="cellDataValue">Current cell's data</param>
        /// <param name="isNewGroupStarted">Indicated starting a new group</param>
        public void CellAdded(object cellDataValue, bool isNewGroupStarted)
        {
            checkNewGroupStarted(isNewGroupStarted);

            if (cellDataValue == null) return;

            var html = cellDataValue.ToString();
            var values = _valueFormatMatch.Matches(html).Cast<Match>()
                                                        .Select(match => match.Groups["V"].Value)
                                                        .ToList();
            var val = float.Parse(values.Last());
            _groupSum += val;
            _overallSum += val;
        }

        /// <summary>
        /// A general method which takes a list of data and calculates its corresponding aggregate value.
        /// It will be used to calculate the aggregate value of each pages individually, with considering the previous pages data.
        /// </summary>
        /// <param name="columnCellsSummaryData">List of data</param>
        /// <returns>Aggregate value</returns>
        public object ProcessingBoundary(IList<SummaryCellData> columnCellsSummaryData)
        {
            if (columnCellsSummaryData == null || !columnCellsSummaryData.Any()) return 0;

            var list = columnCellsSummaryData;

            float sum = 0;
            foreach (var item in list)
            {
                if (item.CellData.PropertyValue == null) continue;

                var html = item.CellData.PropertyValue.ToString();
                var values = _valueFormatMatch.Matches(html).Cast<Match>()
                                                       .Select(match => match.Groups["V"].Value)
                                                       .ToList();
                var val = float.Parse(values.Last());

                sum += val;
            }

            return sum;
        }

        private void checkNewGroupStarted(bool newGroupStarted)
        {
            if (newGroupStarted)
            {
                _groupSum = 0;
            }
        }
    }
}