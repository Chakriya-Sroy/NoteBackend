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

# Copy the published API from build stage
COPY --from=build /app/publish .

# Copy init-db.sql into /app so DbHelper can find it
COPY --from=build /src/init-db.sql .

# Expose port for API
EXPOSE 8080

# Start the API when container runs
ENTRYPOINT ["dotnet", "NoteBackend.dll"]
