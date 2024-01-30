#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Server/WingetNexus.Server.csproj", "Server/"]
COPY ["Client/WingetNexus.Client.csproj", "Client/"]
COPY ["Shared/WingetNexus.Shared.csproj", "Shared/"]
COPY ["Data/WingetNexus.Data.csproj", "Data/"]
RUN dotnet restore "./Server/./WingetNexus.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "./WingetNexus.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WingetNexus.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WingetNexus.Server.dll"]