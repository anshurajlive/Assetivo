# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the project file first and restore dependencies
COPY AssetivoBackend.csproj ./
RUN dotnet restore ./AssetivoBackend.csproj

# Copy the rest of the app and publish it
COPY . ./
RUN dotnet publish ./AssetivoBackend.csproj -c Release -o /app/publish

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Expose the default port (optional, good for local testing)
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "AssetivoBackend.dll"]