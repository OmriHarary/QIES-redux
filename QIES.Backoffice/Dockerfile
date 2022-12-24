FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build
WORKDIR /workspace

COPY src/*.csproj src/
RUN --mount=type=secret,id=nuget,target=/root/.nuget/NuGet/NuGet.Config \
    dotnet restore src/QIES.Backoffice.csproj

COPY src/ src/
WORKDIR /workspace/src
RUN dotnet build -c release --no-restore
RUN dotnet publish -c release --no-build -o /app

FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine
LABEL org.opencontainers.image.source https://github.com/OmriHarary/QIES-redux

WORKDIR /opt/qies
COPY --from=build /app .
ENTRYPOINT ["dotnet", "QIES.Backoffice.dll"]
