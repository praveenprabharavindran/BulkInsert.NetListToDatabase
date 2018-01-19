# Bulk Insert a .NET List to Database

The DataReaderAdapater is an adapter that simplifies bulk insert of any .NET list to the database

## Getting Started

The SqlBulkCopy class provides an efficient means to import data into a SQL Server database. Even so, out of the box, it supports data import only from one of the following types:

* DataTable
* DataRow[]
* IDataReader

 The DataReaderAdapater wraps around the list and exposes itself as an IDataReader that can be directly consumed by the SqlBulkCopy’s ‘WriteToServer’ method.


## Usage

Using the DataReaderAdapter is quite simple:

* Instantiate a DataReaderAdapter with an IList of the type that needs to be persisted.
* Pass the DataReaderAdapter to the SqlBulkCopy.WriteToServer() method.

```csharp
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
```

## Running the end to end tests
 The end to end tests demonstrate a few common scenarios under which the DataReaderAdapter might be used.

### Pre requisites
* Execute the Db script files under the AdapterTests/DbScripts folder on a SQL Server database.
	* Create_Table_Customer.sql
	* Create_Table_Skus.sql
* Modify the connection string in app.config to point to the above database.

### Should InsertListToDb
When the properties of the .NET type and the DB column names match exactly in terms of the names and the order in which they are defined.

### Should InsertListToDbWithColumnMappings
When the names of properties of the .NET type do not match with the DB column names.

In this case, the SqlBulkCopy needs to be provided with a list of column mappings that helps identify which properties go to what columns in DB.

## Built With

* C# .Net

## Acknowledgments

I would like to thank my colleague, Peter Juliano for his expert advice and encouragement.

