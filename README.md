# StoryDev.DBO
StoryDev.DBO is a database object management system primarily used in conjunction with the proprietary software, StoryDev Data Studio.

This library is a simpler approach to modern ORM systems, focusing on scripting capabilities and dynamic database object creation at runtime. This is not intended as a replacement to your typical ORM. However, this can be used in production projects for ASP.NET or other .NET applications.

**Please note that this library is a work-in-progress and features will be added over time.**

## Features
`StoryDev.DBO` is the core library, required for all SQL vendors (except JSON files). All other libraries, `StoryDev.DBO.*` are the relevant vendors for use in your applications.

 * Support for all major SQL vendors: MySQL, SQLite, PostgreSQL, Oracle Database and Microsoft SQL Server.
 * Standard interfaces for managing and performing database actions: `IInstanceManager`, `IDBObject` and `IDBReader`.
 * Access to all bulk queries for `INSERT`, `UPDATE` and `DELETE`.
 * Prepare bulk queries and execute all at once with `Manager.Begin` and `Manager.End`.
 * `ItemConstructor` class for runtime database object creation.
 * `StoryDev.DBO.Scripting` namespace for access to classes and enums to allow you to implement your own scripting or dynamic database runtime.

Note that LINQ queries are not supported at this time.

## Building
`StoryDev.DBO` was built using Visual Studio 2022 and .NET Framework 4.7.2.

The reason not to support .NET Core at this time is due to the use of `System.Reflection.Emit`, which has limited cross-platform capabilities.

**TO BUILD**

Click "Restore NuGet Packages" under the solution in Solution Explorer.
Open the Solution file in Visual Studio and click "Build All" under the Solution Explorer. "Release" mode is the default.

### Known Conflicts
Some database vendors may use different versions of `Google.Protobuf`. If you use more than one database vendor with `StoryDev.DBO`, you may need to update to at least `3.15.0` to resolve any conflicts.

## Usage
Getting started with `StoryDev.DBO` is very simple.

To open a connection:

```cs
IInstanceManager manager = StoryDev.DBO.MySQL.DBObject.Manager;
manager.ConnectionInfo = "Host=localhost;Port=3306;Database=test;Uid=admin;";
var connection = manager.OpenConnection();
manager.CloseConnection(connection);
```

To create a database object, we need to inherit from the respective database vendor.

```cs

class MyTable : StoryDev.DBO.MySQL.DBObject
{
	
}

```

All members that you wish to be queried and captured by the underyling API must be FIELDS, NOT properties.

`StoryDev.DBO` supports all C# basic types, and will include support for arrays soon.

```cs

class MyTable : StoryDev.DBO.MySQL.DBObject
{
	
	[SQLPrimaryKey]
	[SQLAutoIncrement]
	public int UID;

	[SQLStringSize(SQLStringType.Fixed, 256)]
	public string Name;

	[SQLStringSize(SQLStringType.Variable, -1)]
	public string Description;

	public MyTable()
	{
		
	}

}

```

The above attributes are primarily for use when creating tables using `manager.CreateTable`, but this has yet to be implemented and tested.

By creating an instance of your new type, you have access to the following functions: `Insert()`, `Update(filters)`, `Delete()`.

`Insert()`, of course, inserts the database object into the currently active database connection information (previously setup with opening a connection).
`Update()` updates the current instance. If `filters` are provided, then the current instance fields is used to update all rows that match the filters.
`Delete()` naturally deletes the database object.

To search a database, we need to use the `IInstanceManager`. This interface object should be stored for later use, especially if using multiple types of connections simultaneously.

```cs
IEnumerable<MyTable> searchResults = manager.Search<MyTable>();
foreach (MyTable result in searchResults)
{
	// Do something
}
```

If we provide no filters, we get all the rows within that table. But let's say we added a new field called `Type` to our `MyTable` class:

```cs

class MyTable : StoryDev.DBO.MySQL.DBObject
{
	
	[SQLPrimaryKey]
	[SQLAutoIncrement]
	public int UID;

	[SQLStringSize(SQLStringType.Fixed, 256)]
	public string Name;

	[SQLStringSize(SQLStringType.Variable, -1)]
	public string Description;

	public byte Type;

	public MyTable()
	{
		
	}

}

```

And search with some filters:

```cs
IEnumerable<MyTable> searchResults = manager.Search<MyTable>(new DBFilter() {
	FieldName = "Type",
	Operator = DBOperator.Equals,
	ConditionValue = 1
});
foreach (MyTable result in searchResults)
{
	// Do something
}

```

We will get all `MyTable` instances matching the condition, in which case is `Type == 1`.

Currently, LINQ queries are not supported, but with `DBFilter` we can perform unlimited filtering for users without needing to create a LINQ query for every possible condition.

## Roadmap
DBO will be developed over time and will include more features. These will include:

 * Masking options for certain database fields.
 * Support for No-SQL vendors.
 * Non-instanced querying (type-less; purely via scripting or `StoryDev.DBO.Scripting`)
 * DBObject to View mapping options.

## License
This library is licensed under the BSD 2-Clause.

**BSD 2-Clause License**

```
Copyright 2022 StoryDev Studios

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
```
