using FCG.Shared.EventLog.Publisher;
using FCG.Shared.EventService.Contracts.Game;
using FCG.Shared.EventService.Contracts.Library;
using FCG.Shared.EventService.Publisher;
using FCG_Games.Application.Games.DTOs;
using FCG_Games.Application.Games.Requests;
using FCG_Games.Application.Games.Responses;
using FCG_Games.Application.Shared.ExternalServices.Library;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Application.Shared.Results;
using FCG_Games.Domain.Games.Entities;
using FCG_Games.Domain.Games.Enums;
using FCG_Games.Domain.Games.SearchDocuments;
using FCG_Games.Domain.Games.SourcingEvents;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace FCG_Games.Application.Games.Services
{
    public class GameService(IGameRepository repository, 
                             IValidator<GameRequest> validator,
                             IDatabaseSearch<GameDocument> databaseSearch,
                             IEventServicePublisher servicePublisher,
                             IEventLogPublisher eventLogPublisher,
                             IHttpClientFactory httpClient,
                             IConfiguration configuration) : IGameService
    {
        public async Task<Result> CreateGameAsync(GameRequest request, CancellationToken cancellationToken = default)
        {
            var validation = validator.Validate(request);
            if(!validation.IsValid)    
                return Result.Failure(new Error("400", string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

            var id = Guid.NewGuid();
            var gameEntity = Game.Create(id, request.Title, request.Price, request.LaunchYear, request.Developer, request.Genre);
            if (await repository.Exists(gameEntity))
                return Result.Failure<GameResponse>(new Error("409", "Este jogo já está cadastrado"));

            var gameCreatedLogEvent = new GameCreatedEventLog(id, request.Title, request.Price, request.LaunchYear, request.Developer, request.Genre);
            await eventLogPublisher.PublishAsync(gameCreatedLogEvent);

            var evt = new GameCreatedEvent(id, request.Title, request.Price, request.LaunchYear, request.Developer, request.Genre.ToString());
            await servicePublisher.PublishAsync(evt, configuration["ServiceBus:Queues:GamesEvents"]!, "GameCreated");

            return Result.Success();
        }

        public async Task<Result> DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var game = await GetGameByIdAsync(id, cancellationToken);
            if (game is null)
                return Result.Failure<GameResponse>(new Error("404", "Jogo não encontrado"));

            var gameDeletedLogEvent = new GameDeletedEventLog(id);
            await eventLogPublisher.PublishAsync(gameDeletedLogEvent);

            var gameDeletedEvent = new GameDeletedEvent(id);
            await servicePublisher.PublishAsync(gameDeletedEvent, configuration["ServiceBus:Queues:GamesEvents"]!, "GameDeleted");

            var libraryGameDeletedEvent = new LibraryGameDeletedEvent(id);
            await servicePublisher.PublishAsync(libraryGameDeletedEvent, configuration["ServiceBus:Queues:LibrariesEvents"]!, "LibraryGameDeleted");

            return Result.Success(game);
        }

        public async Task<Result<GameResponse>> GetGameByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var game = await databaseSearch.GetDocumentAsync(id);
            if(game is null)
                return Result.Failure<GameResponse>(new Error("404", "Jogo não encontrado"));

            return Result.Success(Parse(game));
        }

        public async Task<Result> UpdateGameAsync(Guid id, GameRequest request, CancellationToken cancellationToken = default)
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid)
                return Result.Failure(new Error("400", string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

            var game = await repository.GetByIdAsync(id, cancellationToken);
            if(game is null)
                return Result.Failure(new Error("404", "Jogo não encontrado"));

            var gameUpdatedEventLog = new GameUpdatedEventLog(id, request.Title, request.Price, request.LaunchYear, request.Developer, request.Genre);
            await eventLogPublisher.PublishAsync(gameUpdatedEventLog);

            var evt = new GameUpdatedEvent(game.Id, request.Title, request.Price, request.LaunchYear, request.Developer, request.Genre.ToString());
            await servicePublisher.PublishAsync(evt, configuration["ServiceBus:Queues:GamesEvents"]!, "GameUpdated");

            return Result.Success();
        }

        public async Task<PagedResult<IEnumerable<GameResponse>>> GetAllGamesAsync(int page, CancellationToken cancellationToken = default)
        {
            var pagination = new Pagination() { Page = page };
            var games = await databaseSearch.GetDocumentsAsync(pagination);
            var response = games.Select(Parse).ToList();
            return PagedResult.Success<IEnumerable<GameResponse>>(response, pagination);
        }

        public async Task<PagedResult<IEnumerable<GameResponse>>> GetCustomizedGamesSearchAsync(int page, Guid userId, string search, CancellationToken cancellationToken = default)
        {
            var userLibrary = await GetUserLibrary(userId, cancellationToken);
            var games = await repository.FilterListByIds([.. userLibrary.Select(d => d.GameId)], cancellationToken);
            var topPurchasedGenders = GetTop3MostPurchaseGenders([.. games]);
            string gendersQuery = string.Concat(topPurchasedGenders.OrderByDescending(d => d.Count).Select(d => d.Genre.ToString()));

            var pagination = new Pagination() { Page = page };
            var gamesDocument = await databaseSearch.GetCustomizedDocumentsAsync(search, gendersQuery, pagination);
            var response = gamesDocument.Select(Parse).ToList();
            return PagedResult.Success<IEnumerable<GameResponse>>(response, pagination);
        }

        public async Task<PagedResult<IEnumerable<GameResponse>>> GetBestSellingGamesAsync(int page, CancellationToken cancellationToken = default)
        {
            var pagination = new Pagination() { Page = page };
            var games = await databaseSearch.GetTopSellersDocumentsAsync(pagination);
            var response = games.Select(Parse).ToList();
            return PagedResult.Success<IEnumerable<GameResponse>>(response, pagination);
        }

        private static GameResponse Parse(GameDocument game)
            => new(game.Id, game.Title, game.Price, game.LaunchYear, game.Developer, (EGenre)Enum.Parse(typeof(EGenre), game.Genre));

        private async Task<List<LibraryResponse>> GetUserLibrary(Guid userId, CancellationToken cancellationToken)
        {
            var userClient = httpClient.CreateClient("LibraryApi");
            var libraryResponse = await userClient.GetAsync($"libraries/user/{userId}", cancellationToken);

            libraryResponse.EnsureSuccessStatusCode();
            var stringResponse = libraryResponse.Content!;
            var userLibrary = await stringResponse.ReadFromJsonAsync<List<LibraryResponse>>(cancellationToken);

            return userLibrary!;
        }

        private static List<MostPurchasedGender> GetTop3MostPurchaseGenders(List<Game> games)
        {
            return [.. games
                .GroupBy(g => g.Genre)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => new MostPurchasedGender(g.Key,g.Count()))];
        }
    }
}
