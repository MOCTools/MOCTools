FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY services/epg-fetcher/MOCTools.EpgFetcher.csproj services/epg-fetcher/
COPY shared/MOCTools.Data/MOCTools.Data.csproj shared/MOCTools.Data/

RUN dotnet restore services/epg-fetcher/MOCTools.EpgFetcher.csproj

COPY services/epg-fetcher/ services/epg-fetcher/
COPY shared/MOCTools.Data/ shared/MOCTools.Data/

RUN dotnet publish \
    services/epg-fetcher/MOCTools.EpgFetcher.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MOCTools.EpgFetcher.dll"]