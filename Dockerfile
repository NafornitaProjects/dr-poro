FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /source

COPY *.sln .
COPY Dr-Poro/ ./Dr-Poro/
RUN dotnet restore

RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Dr-Poro.dll"]
