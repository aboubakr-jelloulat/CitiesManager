# Use SDK for development (no multi-stage needed for local dev)
FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy all project files (they are accessible because context is ./app)
COPY CitiesManager.API/ CitiesManager.API/
COPY CitiesManager.Domain/ CitiesManager.Domain/
COPY CitiesManager.Infrastructure/ CitiesManager.Infrastructure/

# Restore all dependencies
RUN dotnet restore CitiesManager.API/CitiesManager.API.csproj

EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

# Run with hot reload for development
CMD ["dotnet", "watch", "run", "--project", "CitiesManager.API/CitiesManager.API.csproj", "--urls", "http://0.0.0.0:8080"]