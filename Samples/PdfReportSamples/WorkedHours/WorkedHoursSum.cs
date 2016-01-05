using System;
using System.Collections.Generic;
using System.Linq;
using PdfRpt.Core.Contracts;

namespace PdfReportSamples.WorkedHours
{
    public class WorkedHoursSum : IAggregateFunction
    {
        /// <summary>
        /// Fires before rendering of this cell.
        /// Now you have time to manipulate the received object and apply your custom formatting function.
        /// It can be null.
        /// </summary>
        public Func<object, string> DisplayFormatFormula { set; get; }

        #region Fields (2)

        int _groupSum = 0;
        int _overallSum = 0;

        #endregion Fields

        #region Properties (2)

        /// <summary>
        /// Returns current groups' aggregate value.
        /// </summary>
        public object GroupValue
        {
            get { return minToString(_groupSum); }
        }

        /// <summary>
        /// Returns current row's aggregate value without considering the presence of the groups.
        /// </summary>
        public object OverallValue
        {
            get { return minToString(_overallSum); }
        }

        #endregion Properties

        #region Methods (2)

        // Public Methods (1) 

        /// <summary>
        /// Fires after adding a cell to the main table.
        /// </summary>
        /// <param name="cellDataValue">Current cell's data</param>
        /// <param name="isNewGroupStarted">Indicated starting a new group</param>
        public void CellAdded(object cellDataValue, bool isNewGroupStarted)
        {
            checkNewGroupStarted(isNewGroupStarted);

            if (cellDataValue == null) return;

            string cellValue = cellDataValue.ToString();
            var parts = cellValue.Split(':');
            var min = (int.Parse(parts[0]) * 60) + int.Parse(parts[1]);

            _groupSum += min;
            _overallSum += min;
        }
        // Private Methods (1) 

        private void checkNewGroupStarted(bool newGroupStarted)
        {
            if (newGroupStarted)
            {
                _groupSum = 0;
            }
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

            int sum = 0;
            foreach (var item in list)
            {
                if (item.CellData.PropertyValue == null) continue;

                var parts = item.CellData.PropertyValue.ToString().Split(':');
                var min = (int.Parse(parts[0]) * 60) + int.Parse(parts[1]);

                sum += min;
            }

            return minToString(sum);
        }

        #endregion Methods

        string minToString(int min)
        {
            int hours = min / 60;
            int minutes = min - (hours * 60);

            return hours.ToString("00") + ":" + minutes.ToString("00");
        }
    }
}
