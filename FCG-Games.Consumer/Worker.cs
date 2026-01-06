using FCG.Shared.EventService.Consumer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace FCG_Games.Consumer
{
    public class Worker(IServiceScopeFactory scopeFactory, IConfiguration configuration) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly string _queueName = configuration["ServiceBus:Queues:GamesEvents"];

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IQueueConsumer>();

            await consumer.ProcessQueueMessages(_queueName, stoppingToken);
        }
    }
}
