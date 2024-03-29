FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build
WORKDIR /workspace

COPY src/QIES.Api/*.csproj src/QIES.Api/
COPY src/QIES.Core/*.csproj src/QIES.Core/
COPY src/QIES.Infra/*.csproj src/QIES.Infra/
COPY src/QIES.Web/*.csproj src/QIES.Web/
RUN --mount=type=secret,id=nuget,target=/root/.nuget/NuGet/NuGet.Config \
    dotnet restore src/QIES.Web/QIES.Web.csproj

COPY src/QIES.Api/ src/QIES.Api/
COPY src/QIES.Core/ src/QIES.Core/
COPY src/QIES.Infra/ src/QIES.Infra/
COPY src/QIES.Web/ src/QIES.Web/
WORKDIR /workspace/src/QIES.Web
RUN dotnet build -c release --no-restore
RUN dotnet publish -c release --no-build -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
LABEL org.opencontainers.image.source https://github.com/OmriHarary/QIES-redux

WORKDIR /opt/qies
COPY --from=build /app .
ENTRYPOINT ["dotnet", "QIES.Web.dll"]
