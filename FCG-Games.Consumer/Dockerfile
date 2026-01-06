FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Consumer não precisa expor porta, mas vamos deixar para health check se necessário
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Forçar timezone UTC
ENV TZ=UTC
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todos os .csproj primeiro
COPY ["FCG-Games.Consumer/FCG-Games.Consumer.csproj", "FCG-Games.Consumer/"]
COPY ["FCG-Games.Application/FCG-Games.Application.csproj", "FCG-Games.Application/"]
COPY ["FCG-Games.Domain/FCG-Games.Domain.csproj", "FCG-Games.Domain/"]
COPY ["FCG-Games.Infrastructure/FCG-Games.Infrastructure.csproj", "FCG-Games.Infrastructure/"]

# Restaura o Consumer
RUN dotnet restore "FCG-Games.Consumer/FCG-Games.Consumer.csproj"

# Copia todo o resto
COPY . .
WORKDIR "/src/FCG-Games.Consumer"
RUN dotnet build "FCG-Games.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FCG-Games.Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG-Games.Consumer.dll"]