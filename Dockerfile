# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8000
# Koyeb genellikle 8000 portunu kullanır:
ENV ASPNETCORE_URLS=http://+:8000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WaterTracker.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Profil fotoğrafları için klasör
RUN mkdir -p /app/wwwroot/uploads
ENTRYPOINT ["dotnet", "WaterTracker.dll"]
