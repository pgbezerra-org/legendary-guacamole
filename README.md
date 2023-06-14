# legendary-guacamole

This is a multi-tenant royal state service, with a random name for now.

To run this application on your machine, you need .NET 7.0 and MS-SQL Server 2019 or later
From the root folder, run:

```shell
dotnet restore
dotnet watch run --project .\webserver\webserver.csproj
```
This command builds and runs the project, as well as hot-reloads.

To build and run separately, use:

```shell
dotnet build
dotnet run --project .\webserver\webserver.csproj
```
