# Usa la imagen base de .NET 6 SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia el archivo de proyecto y restaura las dependencias
COPY *.csproj .
RUN dotnet restore

# Copia el resto de los archivos y construye la aplicación
COPY . .
RUN dotnet publish -c Release -o /app

# Usa la imagen base de ASP.NET Core Runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copia los archivos publicados desde la etapa de construcción
COPY --from=build /app .

# Configura Seq (opcional, si quieres que Seq también esté en Docker)
# Si usas Seq en otro contenedor, asegúrate de que la URL sea accesible desde este contenedor.
ENV SEQ_URL=http://seq:5341

# Expone el puerto en el que corre la aplicación
EXPOSE 80
EXPOSE 443

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "AgenciaViajes.dll"]