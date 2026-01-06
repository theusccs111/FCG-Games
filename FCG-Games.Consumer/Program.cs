using FCG.Shared.EventService.Consumer;
using FCG.Shared.EventService.Consumer.ServiceBus;
using FCG_Games.Application.Shared;
using FCG_Games.Infrastructure.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FCG_Games.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddConsumerServices(hostContext.Configuration);
                    services.AddHostedService<Worker>();
                });
    }
}
