# Learn more about Dockerfiles: https://docs.docker.com/develop/develop-images/dockerfile_best_practices/
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files
COPY ["Server/Api/Api.csproj", "Server/Api/"]
COPY ["Server/Application/Application.csproj", "Server/Application/"]
COPY ["Server/Domain/Domain.csproj", "Server/Domain/"]
COPY ["Server/Infrastructure/Infrastructure.csproj", "Server/Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Server/Api/Api.csproj"

# Copy all files
COPY . .

# Build application
WORKDIR "/src/Server/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]