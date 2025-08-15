# Teravision - CoP .Net Core

This is a .Net Core Web API.

## Getting Started

``` sh
dotnet restore
```

## Run Web API

``` sh
cd Backend.API

# start server
dotnet run --launch-profile https
```

### Open Swagger page

[https://localhost:8081/swagger](https://localhost:8081/swagger) or [http://localhost:8080/swagger](http://localhost:8080/swagger)


### DB
``` sh
# Create / update the Sqlite database
cd Backend.Service
dotnet ef database update -s ../Backend.API/Backend.API.csproj
```

### Developer https certificate
``` sh
dotnet dev-certs https --trust
```

## Testing

``` sh
cd Backend.Tests

dotnet test
```

## Command Useds

``` sh
# Solution File
dotnet new sln

# Create web api project with controllers
dotnet new webapi --use-controllers -o Backend.API
dotnet sln add ./Backend.API/Backend.API.csproj

# Create library project
dotnet new classlib -o Backend.Service
dotnet sln add ./Backend.Service/Backend.Service.csproj
dotnet add ./Backend.API/Backend.API.csproj reference ./Backend.Service/Backend.Service.csproj

# Create xUnit proyect
dotnet new xunit -o Backend.Tests
dotnet sln add ./Backend.Tests/Backend.Tests.csproj
dotnet add ./Backend.Tests/Backend.Tests.csproj reference ./Backend.API/Backend.API.csproj

```

### EF and migrations

``` sh
# Package to connect with a MS Sql Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# install ef global
dotnet tool install --global dotnet-ef

## following commands execute inside Backend.Service

# drop the database
dotnet ef database drop

# Create a migration
dotnet ef migrations add InitialMigration -s ../Backend.API/Backend.API.csproj

# Removes the last migration
dotnet ef migrations remove -s ../Backend.API/Backend.API.csproj

# Update database
dotnet ef database update -s ../Backend.API/Backend.API.csproj
```

## Docker

To create the Backend Image you can use

``` sh
docker build -t teravision/cop-backend:latest .
```

Some build arguments that can be use are:
- BUILD_CONFIGURATION=[ Debug | Release ]
- USE_APP_HOST=[ true | false ]

``` sh
docker build -t teravision/cop-backend:latest --build-arg BUILD_CONFIGURATION=Release --build-arg USE_APP_HOST=false .
```

### Using compose

To use the docker compose, is required to has a `.env` file, can user the `.env.example` has a start point.

``` sh
docker compose - teravision-cop up -d --build -p
```
