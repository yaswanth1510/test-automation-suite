FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ForexTestSuite.Api/ForexTestSuite.Api.csproj", "src/ForexTestSuite.Api/"]
COPY ["src/ForexTestSuite.Application/ForexTestSuite.Application.csproj", "src/ForexTestSuite.Application/"]
COPY ["src/ForexTestSuite.Domain/ForexTestSuite.Domain.csproj", "src/ForexTestSuite.Domain/"]
COPY ["src/ForexTestSuite.Infrastructure/ForexTestSuite.Infrastructure.csproj", "src/ForexTestSuite.Infrastructure/"]
RUN dotnet restore "src/ForexTestSuite.Api/ForexTestSuite.Api.csproj"
COPY . .
WORKDIR "/src/src/ForexTestSuite.Api"
RUN dotnet build "ForexTestSuite.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ForexTestSuite.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Playwright browsers
RUN apt-get update && apt-get install -y wget gnupg2
RUN dotnet tool install --global Microsoft.Playwright.CLI
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN playwright install --with-deps

# Create logs directory
RUN mkdir -p /app/logs

ENTRYPOINT ["dotnet", "ForexTestSuite.Api.dll"]