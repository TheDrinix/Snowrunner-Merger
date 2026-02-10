#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["apps/api/SnowrunnerMerger.Api.csproj", "./apps/api/"]
RUN dotnet restore "./apps/api/SnowrunnerMerger.Api.csproj"
COPY . .
WORKDIR "/src/apps/api"
RUN dotnet build "./SnowrunnerMerger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Install the EF core global tool
RUN dotnet tool install --global dotnet-ef --version 8.0.6
# Add the EF core global tool to the PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Create the EF core migration bundle
RUN dotnet ef migrations bundle --self-contained -r linux-x64 -o /app/efbundle

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SnowrunnerMerger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY --from=publish /app/efbundle ./efbundle

USER root
RUN chmod +x /app/efbundle
USER app

ENTRYPOINT ["dotnet", "SnowrunnerMerger.Api.dll"]