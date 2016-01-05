using System;
using System.Collections.Generic;
using System.Data.Odbc;
using PdfRpt.Core.Contracts;

namespace PdfRpt.DataSources
{
    /// <summary>
    /// Odbc DataSource
    /// </summary>
    public class OdbcDataReaderDataSource : IDataSource
    {
        #region Fields (3)

        readonly string _connectionString;
        readonly object[] _paramValues;
        readonly string _sql;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Converts the selected records to an IEnumerable of Pdf Cells Data
        /// </summary>
        /// <param name="connectionString">the connection string</param>
        /// <param name="sql">SQL statement to select the required records</param>
        /// <param name="parametersValues">values of the parameters started with @</param>
        public OdbcDataReaderDataSource(string connectionString, string sql, params object[] parametersValues)
        {
            _connectionString = connectionString;
            _sql = sql;
            _paramValues = parametersValues;
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
            using (var odbcConnection = new OdbcConnection(_connectionString))
            {
                using (var odbcCommand = new OdbcCommand(_sql, odbcConnection) { CommandTimeout = 1200 })
                {
                    SqlParametersParser.ApplySafeParameters(odbcCommand, _sql, _paramValues);
                    odbcCommand.Connection.Open();

                    using (var odbcReader = odbcCommand.ExecuteReader())
                    {
                        while (odbcReader.Read())
                        {
                            var result = new List<CellData>();
                            for (var i = 0; i < odbcReader.FieldCount; i++)
                            {
                                var value = odbcReader.GetValue(i);
                                var pdfCellData = new CellData
                                {
                                    PropertyName = odbcReader.GetName(i),
                                    PropertyValue = value == DBNull.Value ? null : value,
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

        #endregion Methods
    }
}
