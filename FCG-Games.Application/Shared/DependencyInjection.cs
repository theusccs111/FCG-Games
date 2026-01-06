using FCG_Games.Application.Games.Requests;
using FCG_Games.Application.Games.Services;
using FCG_Games.Application.Games.Validators;
using FCG_Games.Application.Shared.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FCG_Games.Application.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IValidator<GameRequest>, GameRequestValidator>();

            return services;
        }
    }
}