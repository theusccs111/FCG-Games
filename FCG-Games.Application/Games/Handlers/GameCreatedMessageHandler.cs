using FCG.Shared.EventService.Consumer;
using FCG.Shared.EventService.Contracts.Game;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Domain.Games.Entities;
using FCG_Games.Domain.Games.Enums;
using FCG_Games.Domain.Games.SearchDocuments;
using System.Text.Json;

namespace FCG_Games.Application.Games.Handlers
{
    public class GameCreatedMessageHandler(
        IGameRepository repository,
        IDatabaseSearch<GameDocument> databaseSearch) : IMessageHandler
    {
        public string MessageType => "GameCreated";

        public async Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            GameCreatedEvent gameCreatedEvent = JsonSerializer.Deserialize<GameCreatedEvent>(message)!;

            Game game = Game.Create(gameCreatedEvent.GameId,
                gameCreatedEvent.Title,
                gameCreatedEvent.Price,
                gameCreatedEvent.LaunchYear,
                gameCreatedEvent.Developer,
                (EGenre)Enum.Parse(typeof(EGenre), gameCreatedEvent.Genre, true));

            if(await repository.Exists(game, cancellationToken))
                return;

            await repository.AddAsync(game, cancellationToken);

            var gameDocument = new GameDocument(game.Id, game.Title, game.Developer, game.Genre.ToString(), game.LaunchYear, game.Price);
            await databaseSearch.CreateDocumentAsync(gameDocument);
        }
    }
}
