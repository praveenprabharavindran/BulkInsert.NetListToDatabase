# BulkInsert.NetListToDatabase
Bulk upload .Net lists to a Database

The DataReaderAdapter class presented here is an adapter class that takes in a List (IList<T>) as a constructor parameter and exposes itself as a IDataReader component that can be consumed by the SqlBulkCopy’s ‘WriteToServer’ method.
This works as a real quick way to bulk insert a .net List to the DB with minimal effort.
