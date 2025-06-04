# La Mia Pizzeria CRUD Web API

This repository contains an ASP.NET Core MVC and Web API project that exposes CRUD APIs for managing pizzas. It uses Entity Framework Core with SQL Server.

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- A SQL Server instance accessible by the application

## Building the project

Restore dependencies and build the solution:

```bash
dotnet restore
DOTNET_ROOT=$(dirname $(which dotnet))/..
dotnet build la-mia-pizzeria-crud-mvc.sln
```

## Running the application

Run the MVC/Web API project:

```bash
dotnet run --project la-mia-pizzeria-crud-mvc/la-mia-pizzeria-crud-mvc.csproj
```

The site will start on `https://localhost:5001` (or the next available port). API endpoints are available under `/api/pizzas`.

## Database configuration

The connection string is stored in `appsettings.json` under `ConnectionStrings:PizzeriaContextConnection`:

```json
"ConnectionStrings": {
  "PizzeriaContextConnection": "Data Source=localhost;Initial Catalog=MVCEFPizzeria;Integrated Security=True;TrustServerCertificate=True"
}
```

Update this value to match your SQL Server configuration. After editing the connection string, apply the Entity Framework Core migrations to create the database schema:

```bash
dotnet ef database update --project la-mia-pizzeria-crud-mvc/la-mia-pizzeria-crud-mvc.csproj
```

## Additional notes

- Logs are written to `my-log.txt` in the project directory.
- To run the site using a development configuration, ensure the `ASPNETCORE_ENVIRONMENT` environment variable is set to `Development`.
