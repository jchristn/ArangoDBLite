![alt tag](https://raw.githubusercontent.com/jchristn/ArangoDBLite/main/assets/logo.ico)

# ArangoDBLite

ArangoDBLite is a lightweight SDK for the RESTful API provided by the ArangoDB graph database platform (see https://www.arangodb.com/).

[![NuGet Version](https://img.shields.io/nuget/v/ArangoDBLite.svg?style=flat)](https://www.nuget.org/packages/ArangoDBLite/) [![NuGet](https://img.shields.io/nuget/dt/ArangoDBLite.svg)](https://www.nuget.org/packages/ArangoDBLite) 

## Help, Feedback, Contribute

If you have any issues or feedback, please file an issue here in Github. We'd love to have you help by contributing code for new features, optimization to the existing codebase, ideas for future releases, or fixes!

## Overview

This project was built to provide a dead simple interface to interact with ArangoDB.  It is not meant to be complete, but rather, optimized for the simplest possible workflows and integration.

## New in v1.0.x

- Initial release
- Fix for ```First``` APIs
- Include cursor query in log messages

## Example Project

Refer to the ```Test``` project for exercising each of the library APIs.

## Simple Example

All APIs (see list below) return an object of type ```AdbResult```, which contains many of the properties found in the Arango RESTful API response.

The response data can be found in ```AdbResult.Result``` (which should be cast to the appropriate type), ```AdbResult.Graphs``` (a list of ```AdbGraph``` objects), or ```AdbResult.Graph``` (an individual ```AdbGraph``` object).

Refer to the documentation for each API to understand the type of object contained in ```AdbResult.Result```.

```csharp
using ArangoDBLite;

AdbClient client = new AdbClient("root", "root", "http://127.0.0.1:8529/");
AdbResult result = null;

// list databases
result = await client.ListDatabases();
List<string> dbNames = result.Result as List<string>;

// add database
result = await client.CreateDatabase("MyDatabase");
bool success = result.Result as bool;

// get users
result = await client.ListUsers();
List<AdbUser> users = result.Result as List<AdbUser>;

// execute a cursor query
result = await client.ExecuteCursorQuery<AdbVertex>("FOR v IN vertices RETURN v");
List<AdbVertex> vertices = result.Result as List<AdbVertex>;
```

## APIs Provided

The following APIs are provided:

- Database - list, exists, create, delete
- Users - list, get, exists, create, delete
- Collections - list, get, exists, create, delete
- Graphs - list, get, exists, create, delete, add edge collection
- Vertices - list (using cursor query), create, update, replace, get, delete, first
- Edges - list (using cursor query), create, update, replace, get, delete, first
- Cursor queries - execute

## Version History

Refer to CHANGELOG.md for version history.
