FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/AvaliacaoBackend.Api/AvaliacaoBackend.Api.csproj", "src/AvaliacaoBackend.Api/"]
RUN dotnet restore "src/AvaliacaoBackend.Api/AvaliacaoBackend.Api.csproj"
COPY . .
RUN dotnet publish "src/AvaliacaoBackend.Api/AvaliacaoBackend.Api.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AvaliacaoBackend.Api.dll"]
