# ---- STAGE 1: Build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything into the build context
COPY . .

# Restore dependencies
RUN dotnet restore

# Publish the app to /app/publish (Release mode)
RUN dotnet publish -c Release -o /app/publish

# ---- STAGE 2: Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app


RUN apt-get update && \
    apt-get install -y curl apt-transport-https gnupg && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/debian/11/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y msodbcsql18 unixodbc-dev && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Copy the published API from build stage
COPY --from=build /app/publish .

# Copy init-db.sql into /app so DbHelper can find it
COPY --from=build /src/init-db.sql .

# Expose port for API
EXPOSE 8080

# Start the API when container runs
ENTRYPOINT ["dotnet", "NoteBackend.dll"]