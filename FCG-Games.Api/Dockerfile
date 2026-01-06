FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Install the new relic agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y 'newrelic-dotnet-agent' \
&& rm -rf /var/lib/apt/lists/*

# Enable the new relic agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=dddb37f9ffc2d5698f2dd3037e1e7487FFFFNRAL \
NEW_RELIC_APP_NAME="FIAP-Games"

WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Forçar timezone UTC
ENV TZ=UTC
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todos os .csproj primeiro
COPY ["FCG-Games.Api/FCG-Games.Api.csproj", "FCG-Games.Api/"]
COPY ["FCG-Games.Application/FCG-Games.Application.csproj", "FCG-Games.Application/"]
COPY ["FCG-Games.Domain/FCG-Games.Domain.csproj", "FCG-Games.Domain/"]
COPY ["FCG-Games.Infrastructure/FCG-Games.Infrastructure.csproj", "FCG-Games.Infrastructure/"]

# Restaura apenas a API
RUN dotnet restore "FCG-Games.Api/FCG-Games.Api.csproj"

# Copia todo o resto
COPY . .

WORKDIR "/src/FCG-Games.Api"
RUN dotnet build "FCG-Games.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FCG-Games.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG-Games.Api.dll"]