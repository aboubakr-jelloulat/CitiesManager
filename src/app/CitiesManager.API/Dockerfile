# Stage 1: base runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

# Stage 2: restore (cached layer)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src
COPY CitiesManager.Web.csproj .
RUN dotnet restore "CitiesManager.Web.csproj"

# Stage 3: build
FROM restore AS build
ARG BUILD_CONFIGURATION=Release
COPY . .
RUN dotnet build "CitiesManager.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore

# Stage 4: publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CitiesManager.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Stage 5: final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CitiesManager.Web.dll"]