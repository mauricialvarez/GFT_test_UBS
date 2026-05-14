# syntax=docker/dockerfile:1

ARG DOTNET_VERSION=8.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine AS restore
WORKDIR /src

COPY GFT_test_UBS.sln ./
COPY src/GFT_test_UBS.Domain/GFT_test_UBS.Domain.csproj src/GFT_test_UBS.Domain/
COPY src/GFT_test_UBS.Application/GFT_test_UBS.Application.csproj src/GFT_test_UBS.Application/
COPY src/GFT_test_UBS.Infrastructure/GFT_test_UBS.Infrastructure.csproj src/GFT_test_UBS.Infrastructure/
COPY src/GFT_test_UBS.Interface/GFT_test_UBS.Interface.csproj src/GFT_test_UBS.Interface/
COPY test/GFT_test_UBS.Domain.Tests/GFT_test_UBS.Domain.Tests.csproj test/GFT_test_UBS.Domain.Tests/
COPY test/GFT_test_UBS.Application.Tests/GFT_test_UBS.Application.Tests.csproj test/GFT_test_UBS.Application.Tests/
COPY test/GFT_test_UBS.Infrastructure.Tests/GFT_test_UBS.Infrastructure.Tests.csproj test/GFT_test_UBS.Infrastructure.Tests/
COPY test/GFT_test_UBS.Interface.Tests/GFT_test_UBS.Interface.Tests.csproj test/GFT_test_UBS.Interface.Tests/
COPY benchmarks/GFT_test_UBS.Benchmarks/GFT_test_UBS.Benchmarks.csproj benchmarks/GFT_test_UBS.Benchmarks/

RUN dotnet restore GFT_test_UBS.sln

FROM restore AS build
COPY . .
RUN dotnet build GFT_test_UBS.sln \
    --configuration Release \
    --no-restore

FROM build AS test
RUN dotnet test GFT_test_UBS.sln \
    --configuration Release \
    --no-build

FROM test AS publish
RUN dotnet publish src/GFT_test_UBS.Interface/GFT_test_UBS.Interface.csproj \
    --configuration Release \
    --no-build \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:${DOTNET_VERSION}-alpine AS runtime
WORKDIR /app

RUN addgroup -S gftapp \
    && adduser -S gftapp -G gftapp

COPY --from=publish /app/publish .

USER gftapp
ENTRYPOINT ["dotnet", "GFT_test_UBS.Interface.dll"]
