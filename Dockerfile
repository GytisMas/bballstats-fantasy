FROM mcr.microsoft.com/dotnet/core/sdk:8.0-alpine AS build
WORKDIR /src
COPY . .

RUN dotnet restore "bballstats-fantasy/BBallStatsV2.Web.csproj"
WORKDIR "/src/."
COPY . .
RUN dotnet build "bballstats-fantasy/BBallStatsV2.Web.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "bballstats-fantasy/BBallStatsV2.Web.csproj" -c Release -o /app/publish

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