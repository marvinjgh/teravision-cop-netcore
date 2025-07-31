# Teravision - CoP .Net Core

This is a .net Core API.

## Getting Started

``` sh
dotnet restore
```

## Run Web API

``` sh
cd Backend.API

dotnet run --launch-profile https
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
