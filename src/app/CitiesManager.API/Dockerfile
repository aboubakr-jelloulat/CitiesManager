# base runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


RUN groupadd --system appgroup && \
    useradd --system --gid appgroup --no-create-home appuser

USER appuser

# restore : Package Download
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src
COPY CitiesManager.API.csproj .
RUN dotnet restore "CitiesManager.API.csproj"

# build
FROM restore AS build
ARG BUILD_CONFIGURATION=Release
COPY . .
RUN dotnet build "CitiesManager.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore

# publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CitiesManager.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CitiesManager.API.dll"]