FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY . .

# copy csproj and restore as distinct layers
COPY src/BBallStatsV2/*.csproj .
COPY src/BBallStats.Shared/. .
RUN dotnet restore "src/BBallStatsV2/BBallStatsV2.csproj" -r linux-musl-x64 /p:PublishReadyToRun=true

# copy everything else and build app
COPY src/. .
RUN dotnet publish "src/BBallStatsV2/BBallStatsV2.csproj" -c Release -o /app -r linux-musl-x64 --self-contained true --no-restore /p:PublishReadyToRun=true /p:PublishSingleFile=true

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine-amd64
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./BBallStatsV2"]

# See: https://github.com/dotnet/announcements/issues/20
# Uncomment to enable globalization APIs (or delete)
ENV \
     DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
     LC_ALL=en_US.UTF-8 \
     LANG=en_US.UTF-8
 RUN apk add --no-cache \
     icu-data-full \
     icu-libs