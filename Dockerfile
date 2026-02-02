FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DrPoro.sln .
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY global.json .

COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Client/Client.csproj src/Client/

RUN dotnet restore DrPoro.sln

COPY src ./src

RUN dotnet publish src/Client/Client.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DrPoro.Client.dll"]
