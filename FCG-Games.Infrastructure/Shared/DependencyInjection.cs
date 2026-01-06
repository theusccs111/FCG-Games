using Azure.Messaging.ServiceBus;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using FCG.Shared.EventLog.Publisher;
using FCG.Shared.EventLog.Publisher.MongoDB;
using FCG.Shared.EventService.Publisher;
using FCG.Shared.EventService.Publisher.ServiceBus;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Domain.Games.SearchDocuments;
using FCG_Games.Infrastructure.Games.DatabaseSearch;
using FCG_Games.Infrastructure.Games.Repositories;
using FCG_Games.Infrastructure.Shared.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FCG_Games.Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GamesDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            
            var connectionStringServiceBus = configuration["ServiceBus:ConnectionString"];
            services.AddSingleton(new ServiceBusClient(connectionStringServiceBus));

            var connectionStringMongodb = configuration["Mongodb:ConnectionString"];
            services.AddSingleton(new MongoClient(connectionStringMongodb));

            services.AddScoped<IEventServicePublisher>(sp =>
            {
                var client = sp.GetRequiredService<ServiceBusClient>();
                return new ServiceBusEventPublisher(client);
            });

            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IEventLogPublisher>(sp =>
            {
                var client = sp.GetRequiredService<MongoClient>();
                var databaseName = configuration["Mongodb:DatabaseName"];
                var collectionName = configuration["Mongodb:CollectionName"];

                return new MongoDBEventLogPublisher(client, databaseName!, collectionName!);
            });

            var elasticSearchUrl = configuration["ElasticSearch:Url"]!;
            var elasticSearchApiKey = configuration["ElasticSearch:ApiKey"]!;
            services.AddSingleton(sp =>
            {
                var settings = new ElasticsearchClientSettings(new Uri(elasticSearchUrl))
                    .Authentication(new ApiKey(elasticSearchApiKey));
                return new ElasticsearchClient(settings);
            });
            services.AddScoped<IDatabaseSearch<GameDocument>, GameDatabaseSearch>();

            return services;
        }
    }
}
