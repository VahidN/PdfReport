using System.Collections.Generic;
using System.Data;
using PdfRpt.Core.Contracts;

namespace PdfRpt.DataSources
{
    /// <summary>
    /// System.Data.DataTable DataSource
    /// </summary>
    public class DataTableDataSource : IDataSource
    {
        #region Fields (2)

        readonly int _columnsCount;
        readonly DataTable _dataTable;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Converts a DataTable to an IEnumerable of Pdf Cells Data
        /// </summary>
        /// <param name="dataTable">our dataTable value to convert</param>
        public DataTableDataSource(DataTable dataTable)
        {
            _dataTable = dataTable;
            _columnsCount = _dataTable.Columns.Count;
        }

        #endregion Constructors

        #region Methods (1)

        // Public Methods (1) 

        /// <summary>
        /// The data to render.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IList<CellData>> Rows()
        {
            if (_dataTable == null) yield break;

            foreach (DataRow row in _dataTable.Rows)
            {
                var list = new List<CellData>();
                for (var i = 0; i < _columnsCount; i++)
                {
                    var pdfCellData = new CellData
                    {
                        PropertyName = _dataTable.Columns[i].ColumnName,
                        PropertyValue = row[i],
                        PropertyIndex = i
                    };
                    list.Add(pdfCellData);
                }
                yield return list;
            }
        }

        #endregion Methods
    }
}
