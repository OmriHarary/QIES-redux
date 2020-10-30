FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /workspace

COPY src/QIES.Api/*.csproj src/QIES.Api/
COPY src/QIES.Core/*.csproj src/QIES.Core/
COPY src/QIES.Infra/*.csproj src/QIES.Infra/
COPY src/QIES.Web/*.csproj src/QIES.Web/
RUN dotnet restore src/QIES.Web/QIES.Web.csproj

COPY src/QIES.Api/ src/QIES.Api/
COPY src/QIES.Core/ src/QIES.Core/
COPY src/QIES.Infra/ src/QIES.Infra/
COPY src/QIES.Web/ src/QIES.Web/
WORKDIR /workspace/src/QIES.Web
RUN dotnet build -c release --no-restore
RUN dotnet publish -c release --no-build -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0
LABEL org.opencontainers.image.source https://github.com/OmriHarary/QIES-redux

WORKDIR /opt/qies
COPY --from=build /app .
ENTRYPOINT ["dotnet", "QIES.Web.dll"]
