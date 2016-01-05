using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using PdfRpt.Core.Contracts;

namespace PdfRpt.DataSources
{
    /// <summary>
    /// Using the data provider factory classes to develop a pluggable data layer 
    /// that is independent of database type and ADO.NET data provider. 
    /// </summary>
    public class GenericDataReaderDataSource : IDataSource
    {
        #region Fields (3)

        readonly string _connectionString;
        readonly object[] _paramValues;
        readonly string _sql;
        readonly string _providerName;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Converts the selected records to an IEnumerable of the Pdf Cells Data
        /// </summary>
        /// <param name="providerName">Invariant name of a provider. Supports the ADO.NET Factory classes to allow you the ability to dynamically load the provider at runtime</param>
        /// <param name="connectionString">the connection string</param>
        /// <param name="sql">SQL statement to select the required records</param>
        /// <param name="parametersValues">values of the parameters started with @</param>
        public GenericDataReaderDataSource(string providerName, string connectionString, string sql, params object[] parametersValues)
        {
            _providerName = providerName;
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
            var factory = DbProviderFactories.GetFactory(_providerName);
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = _sql;
                    command.CommandType = CommandType.Text;

                    SqlParametersParser.ApplySafeParameters(command, _sql, _paramValues);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var result = new List<CellData>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.GetValue(i);
                                var pdfCellData = new CellData
                                {
                                    PropertyName = reader.GetName(i),
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
