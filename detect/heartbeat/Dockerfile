FROM mcr.microsoft.com/dotnet/runtime:3.1-focal AS base
# FROM diamol/dotnet-runtime:linux-arm64 AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:3.1-focal AS build
WORKDIR /src
COPY ["heartbeat.csproj", "detect/heartbeat/"]
RUN dotnet restore "detect/heartbeat/heartbeat.csproj"
COPY . .
WORKDIR "/src/detect/heartbeat"
RUN dotnet build "heartbeat.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "heartbeat.csproj" -c Release -o /app/publish -r linux-armv8

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "heartbeat.dll"]
