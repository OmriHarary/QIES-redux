FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /workspace

COPY src/*.csproj src/
RUN dotnet restore src/QIES.Backoffice.csproj

COPY src/ src/
WORKDIR /workspace/src
RUN dotnet build -c release --no-restore
RUN dotnet publish -c release --no-build -o /app

FROM mcr.microsoft.com/dotnet/runtime:5.0
LABEL org.opencontainers.image.source https://github.com/OmriHarary/QIES-redux

WORKDIR /opt/qies
COPY --from=build /app .
ENTRYPOINT ["dotnet", "QIES.Backoffice.dll"]
