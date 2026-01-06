using Azure.Messaging.ServiceBus;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using FCG.Shared.EventService.Consumer;
using FCG.Shared.EventService.Consumer.ServiceBus;
using FCG_Games.Application.Games.Handlers;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Domain.Games.SearchDocuments;
using FCG_Games.Infrastructure.Games.DatabaseSearch;
using FCG_Games.Infrastructure.Games.Repositories;
using FCG_Games.Infrastructure.Shared.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FCG_Games.Consumer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddConsumerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GamesDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            var connectionStringServiceBus = configuration["ServiceBus:ConnectionString"];
            var queueName = configuration["ServiceBus:Queues:GamesEvents"];

            services.AddSingleton(new ServiceBusClient(connectionStringServiceBus));

            services.AddScoped<IGameRepository, GameRepository>();

            services.AddScoped<IMessageHandler, GameCreatedMessageHandler>();
            services.AddScoped<IMessageHandler, GameUpdatedMessageHandler>();
            services.AddScoped<IMessageHandler, GameDeletedMessageHandler>();
            services.AddScoped<IMessageHandler, GameSoldMessageHandler>();

            services.AddScoped<IQueueConsumer, QueueConsumer>();
            services.AddScoped<ServiceBusMessageDispatcher>();

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
