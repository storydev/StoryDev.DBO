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
