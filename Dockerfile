# Usa la imagen SDK de .NET para compilar la aplicación
# Esto se conoce como una "etapa de compilación"
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia el archivo de la solución y los archivos de proyectos para restaurar las dependencias
COPY ["*.sln", "./"]
COPY ["Backend.API/Backend.API.csproj", "Backend.API/"]
RUN dotnet restore "Backend.API/Backend.API.csproj"

# Copia el resto del código y compila el proyecto
COPY . .
WORKDIR "/app/Backend.API"
RUN dotnet build "Backend.API.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "Backend.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Usa la imagen de runtime de ASP.NET para la aplicación final
# Esta es una "etapa de producción" que es más ligera y segura
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Define el puerto que la aplicación escuchará. App Platform lo expondrá.
ENV ASPNETCORE_URLS=http://+:8080

# Inicia la aplicación
ENTRYPOINT ["dotnet", "Backend.API.dll"]
