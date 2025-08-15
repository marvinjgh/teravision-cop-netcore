FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Usa la imagen SDK de .NET para compilar la aplicación (etapa de compilación)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app

# Copia los archivos de la solución y los proyectos para restaurar dependencias
# Corrige el nombre del archivo de la solución para que coincida con tu repositorio
COPY ["Backend.API/Backend.API.csproj", "Backend.API/"]
COPY ["Backend.Service/Backend.Service.csproj", "Backend.Service/"]
# Restaura todas las dependencias de la solución
RUN dotnet restore "Backend.API/Backend.API.csproj"

# Copia el resto del código y compila el proyecto de la API
COPY . .
WORKDIR "/app/Backend.API"
RUN dotnet build "Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica la aplicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG USE_APP_HOST=false
RUN dotnet publish "Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=$USE_APP_HOST

# Usa la imagen de runtime de ASP.NET para la aplicación final (etapa de producción)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.API.dll"]