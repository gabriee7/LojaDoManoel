FROM mcr.microsoft.com/dotnet/sdk:9.0 AS sdk-base
WORKDIR /src

COPY BoxOptimizerMicroservice.sln ./
COPY NuGet.Config ./

COPY BoxOptimizerMicroservice/ ./BoxOptimizerMicroservice/
COPY BoxOptimizerMicroservice.Tests/ ./BoxOptimizerMicroservice.Tests/

ENV NUGET_PACKAGES_PATH=/usr/share/nuget/packages
RUN dotnet restore BoxOptimizerMicroservice.sln --configfile NuGet.Config --runtime linux-x64 --packages ${NUGET_PACKAGES_PATH} --force-evaluate


FROM sdk-base AS build-api
WORKDIR /src/BoxOptimizerMicroservice
RUN dotnet publish BoxOptimizerMicroservice.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    --runtime linux-x64 \
    /p:UseAppHost=false \
    /p:DisableImplicitNuGetFallbackFolder="true"


FROM sdk-base AS test-runner 
WORKDIR /src/BoxOptimizerMicroservice.Tests

RUN dotnet test BoxOptimizerMicroservice.Tests.csproj \
    -c Release \
    --no-restore \
    --results-directory /app/test-results

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final-api
WORKDIR /app
ENV LANG C.UTF-8
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY --from=build-api /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "BoxOptimizerMicroservice.dll"]