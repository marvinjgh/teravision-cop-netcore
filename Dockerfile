# Usa la imagen SDK de .NET para compilar la aplicación (etapa de compilación)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia los archivos de la solución y los proyectos para restaurar dependencias
# Corrige el nombre del archivo de la solución para que coincida con tu repositorio
COPY ["teravision-cop-backend.sln", "./"]
COPY ["Backend.API/Backend.API.csproj", "Backend.API/"]
# Restaura todas las dependencias de la solución
RUN dotnet restore "teravision-cop-backend.sln"

# Copia el resto del código y compila el proyecto de la API
COPY . .
WORKDIR "/app/Backend.API"
RUN dotnet build "Backend.API.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "Backend.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Usa la imagen de runtime de ASP.NET para la aplicación final (etapa de producción)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Define el puerto que la aplicación escuchará
ENV ASPNETCORE_URLS=http://+:8080

# Inicia la aplicación
ENTRYPOINT ["dotnet", "Backend.API.dll"]