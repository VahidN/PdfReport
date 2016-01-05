using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using PdfRpt.Core.Contracts;
using PdfRpt.DataSources;

namespace PdfRpt.FluentInterface
{
    /// <summary>
    /// Main Table DataSource Builder.
    /// </summary>
    public class MainTableDataSourceBuilder
    {
        readonly PdfReport _pdfReport;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="pdfReport"></param>
        public MainTableDataSourceBuilder(PdfReport pdfReport)
        {
            _pdfReport = pdfReport;
        }

        /// <summary>
        /// Converts the XML documents data to an IEnumerable of Pdf Cells Data
        /// </summary>
        /// <param name="xmlData">XML document's content</param>
        /// <param name="descendantsXPathSelect">Descendants XPath</param>
        /// <param name="itemsXPathList">XPath list of the required items</param>
        public void Xml(string xmlData, string descendantsXPathSelect, IList<string> itemsXPathList)
        {
            CustomDataSource(() => new XmlDataSource(xmlData, descendantsXPathSelect, itemsXPathList));
        }

        /// <summary>
        /// Converts a list of strongly typed items to an IEnumerable of Pdf Cells Data.
        /// It's useful for working with different ORM's, because final results of all them could be 
        /// a strongly typed list of data.
        /// </summary>
        /// <param name="listOfRows">list of items</param>
        /// <param name="dumpLevel">how many levels should be searched</param>
        public void StronglyTypedList<T>(IEnumerable<T> listOfRows, int dumpLevel = 2) where T : class
        {
            CustomDataSource(() => new StronglyTypedListDataSource<T>(listOfRows, dumpLevel));
        }

        /// <summary>
        /// Converts the selected records to an IEnumerable of the Pdf Cells Data
        /// </summary>
        /// <param name="connectionString">the connection string</param>
        /// <param name="sql">SQL statement to select the required records</param>
        /// <param name="parametersValues">values of the parameters started with @</param>
        public void SqlDataReader(string connectionString, string sql, params object[] parametersValues)
        {
            CustomDataSource(() => new SqlDataReaderDataSource(connectionString, sql, parametersValues));
        }

        /// <summary>
        /// Converts the selected records to an IEnumerable of the Pdf Cells Data
        /// </summary>
        /// <param name="providerName">Invariant name of a provider. Supports the ADO.NET Factory classes to allow you the ability to dynamically load the provider at runtime</param>
        /// <param name="connectionString">the connection string</param>
        /// <param name="sql">SQL statement to select the required records</param>
        /// <param name="parametersValues">values of the parameters started with @</param>
        public void GenericDataReader(string providerName, string connectionString, string sql, params object[] parametersValues)
        {
            CustomDataSource(() => new GenericDataReaderDataSource(providerName, connectionString, sql, parametersValues));
        }

        /// <summary>
        /// Converts the selected records to an IEnumerable of Pdf Cells Data
        /// </summary>
        /// <param name="connectionString">the connection string</param>
        /// <param name="sql">SQL statement to select the required records</param>
        /// <param name="parametersValues">values of the parameters started with @</param>
        public void OdbcDataReader(string connectionString, string sql, params object[] parametersValues)
        {
            CustomDataSource(() => new OdbcDataReaderDataSource(connectionString, sql, parametersValues));
        }

        /// <summary>
        /// Converts the selected records to an IEnumerable of Pdf Cells Data
        /// </summary>
        /// <param name="filePath">.xlsx or .xls file path</param>
        /// <param name="sql">SQL statement to select the required records</param>
        /// <param name="parametersValues">values of the parameters started with @</param>
        public void ExcelDataReader(string filePath, string sql, params object[] parametersValues)
        {
            CustomDataSource(() => new ExcelDataReaderDataSource(filePath, sql, parametersValues));
        }

        /// <summary>
        /// Converts a DataTable to an IEnumerable of Pdf Cells Data
        /// </summary>
        /// <param name="rows">our dataTable value to convert</param>
        public void DataTable(DataTable rows)
        {
            CustomDataSource(() => new DataTableDataSource(rows));
        }

        /// <summary>
        /// Converts the result of the CrosstabExtension.Pivot method to an IEnumerable of Pdf Cells Data
        /// </summary>
        /// <param name="source">Result of the CrosstabExtension.Pivot method</param>
        /// <param name="topFieldsAreVariableInEachRow">Indicates whether top fields should be prepopulated before starting the main table's rendering or not</param>
        /// <param name="dumpLevel">how many levels should be searched</param>  
        public void Crosstab(IEnumerable source, bool topFieldsAreVariableInEachRow = false, int dumpLevel = 2)
        {
            CustomDataSource(() => new CrosstabDataSource(source, topFieldsAreVariableInEachRow, dumpLevel));
        }

        /// <summary>
        /// Converts a list of anonymous type items to an IEnumerable of Pdf Cells Data.
        /// </summary>
        /// <param name="listOfRows">list of items</param>
        /// <param name="dumpLevel">how many levels should be searched</param>
        public void AnonymousTypeList(IEnumerable listOfRows, int dumpLevel = 2)
        {
            CustomDataSource(() => new AnonymousTypeListDataSource(listOfRows, dumpLevel));
        }

        /// <summary>
        /// Converts the selected records to an IEnumerable of the Pdf Cells Data
        /// </summary>
        /// <param name="filePath">.mdb or .accdb file path</param>
        /// <param name="password">the optional password</param>
        /// <param name="sql">SQL statement to select the required records</param>
        /// <param name="parametersValues">values of the parameters started with @</param>
        public void AccessDataReader(string filePath, string password, string sql, params object[] parametersValues)
        {
            CustomDataSource(() => new AccessDataReaderDataSource(filePath, password, sql, parametersValues));
        }

        /// <summary>
        /// Main table's data source. The data to render. 
        /// </summary>
        /// <param name="mainTableDataSource">Main table's data source</param>
        public void CustomDataSource(Func<IDataSource> mainTableDataSource)
        {
            _pdfReport.DataBuilder.SetMainTableDataSource(mainTableDataSource);
        }
    }
}
