using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using PdfRpt.Core.Contracts;
using System.Globalization;

namespace PdfRpt.DataSources
{
    /// <summary>
    /// Microsoft Excel Worksheet Reader DataSource
    /// </summary>
    public class ExcelDataReaderDataSource : IDataSource
    {
        #region Fields (3)

        readonly string _filePath;
        readonly object[] _paramValues;
        readonly string _sql;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Converts the selected records to an IEnumerable of Pdf Cells Data
        /// </summary>
        /// <param name="filePath">.xlsx or .xls file path</param>
        /// <param name="sql">SQL statement to select the required records</param>
        /// <param name="parametersValues">values of the parameters started with @</param>
        public ExcelDataReaderDataSource(string filePath, string sql, params object[] parametersValues)
        {
            _filePath = filePath;
            _sql = sql;
            _paramValues = parametersValues;
        }

        #endregion Constructors

        #region Methods (2)

        // Public Methods (1) 

        /// <summary>
        /// The data to render.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IList<CellData>> Rows()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException(_filePath + " file not found.");

            var connectionString = getConnectionString();

            using (var oleDbConnection = new OleDbConnection(connectionString))
            {
                using (var oleDbCommand = new OleDbCommand(_sql, oleDbConnection) { CommandTimeout = 1200 })
                {
                    SqlParametersParser.ApplySafeParameters(oleDbCommand, _sql, _paramValues);
                    oleDbCommand.Connection.Open();

                    using (var oleDbReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbReader != null && oleDbReader.Read())
                        {
                            var result = new List<CellData>();
                            for (var i = 0; i < oleDbReader.FieldCount; i++)
                            {
                                var value = oleDbReader.GetValue(i);
                                var pdfCellData = new CellData
                                {
                                    PropertyName = oleDbReader.GetName(i),
                                    PropertyValue = (value == DBNull.Value) ? null : value,
                                    PropertyIndex = i
                                };
                                result.Add(pdfCellData);
                            }
                            yield return result;
                        }
                    }
                }
            }
        }
        // Private Methods (1) 

        private string getConnectionString()
        {
            var connectionString = string.Format(CultureInfo.InvariantCulture, "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0};Extended Properties=\"Excel 8.0;HDR=YES\"", _filePath);
            if (Path.GetExtension(_filePath).ToUpperInvariant() == ".XLSX")
            {
                connectionString = string.Format(CultureInfo.InvariantCulture, "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;IMEX=1;HDR=YES;\"", _filePath);
            }
            return connectionString;
        }

        #endregion Methods
    }
}
