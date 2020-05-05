FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY FleetPing.Api/FleetPing.Api.csproj FleetPing.Api/
RUN dotnet restore FleetPing.Api/FleetPing.Api.csproj

COPY FleetPing.Api/ FleetPing.Api/
RUN dotnet publish FleetPing.Api/FleetPing.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "FleetPing.Api.dll"]
