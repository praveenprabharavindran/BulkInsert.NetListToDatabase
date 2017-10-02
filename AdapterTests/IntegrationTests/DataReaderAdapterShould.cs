using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace AdapterTests.IntegrationTests
{
    [TestClass]
    public class DataReaderAdapterShould
    {
        private readonly string _connectionString;

        private readonly Fixture _fixture = new Fixture();

        public DataReaderAdapterShould()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ToString();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void InsertListToDb()
        {
            //Generate a list of 10,000 Customer records
            var customers = _fixture.CreateMany<Customer>(10000).ToList();
            var customerDr = new DataReaderAdapter<Customer>(customers);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = "[dbo].[Customer]",
                    BatchSize = 1000
                };
                bulkCopy.WriteToServer(customerDr);
                bulkCopy.Close();
            }
        }

        [TestMethod]
        public void InsertListToDbWithColumnMappings()
        {
            var skus = _fixture.CreateMany<Sku>(10000).ToList();
            var customerReader = new DataReaderAdapter<Sku>(skus);

            //Get column mappings from the 'Sku' class to DB
            var columnMappings = GetColumnMappings(customerReader);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = "[dbo].[Skus]",
                    BatchSize = 1000
                };

                //Add the column mappings to the bulkCopy object
                foreach (var columnName in columnMappings)
                {
                    bulkCopy.ColumnMappings.Add(columnName.Item1, columnName.Item2);
                }
                bulkCopy.WriteToServer(customerReader);
                bulkCopy.Close();
            }
        }

        /// <summary>
        ///     Prepare a column mappings for the 'Sku' class using the adapter's ColumnNames property
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        private List<Tuple<string, string>> GetColumnMappings(DataReaderAdapter<Sku> adapter)
        {
            var sourceColumnNames = adapter.ColumnNames;
            var columnMappings = new List<Tuple<string, string>>();
            foreach (var columnName in sourceColumnNames)
            {
                Tuple<string, string> currentMapping;
                switch (columnName.ToUpper())
                {
                    case "ID":
                        currentMapping = new Tuple<string, string>(columnName, "s_id");
                        break;
                    case "NAME":
                        currentMapping = new Tuple<string, string>(columnName, "s_name");
                        break;
                    case "PRICE":
                        currentMapping = new Tuple<string, string>(columnName, "s_price");
                        break;
                    case "CATEGORY":
                        currentMapping = new Tuple<string, string>(columnName, "s_category");
                        break;
                    default:
                        currentMapping = new Tuple<string, string>(columnName, columnName);
                        break;
                }
                columnMappings.Add(currentMapping);
            }
            return columnMappings;
        }
    }
}