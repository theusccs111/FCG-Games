using FCG.Shared.EventService.Consumer;
using FCG.Shared.EventService.Contracts.Game;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Domain.Games.Enums;
using FCG_Games.Domain.Games.SearchDocuments;
using System.Text.Json;

namespace FCG_Games.Application.Games.Handlers
{
    public class GameUpdatedMessageHandler(
        IGameRepository repository,
        IDatabaseSearch<GameDocument> databaseSearch) : IMessageHandler
    {
        public string MessageType => "GameUpdated";

        public async Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            GameUpdatedEvent gameUpdatedEvent = JsonSerializer.Deserialize<GameUpdatedEvent>(message)!;
            var game = await repository.GetByIdAsync(gameUpdatedEvent.GameId, cancellationToken);

            if (game == null)
                return;

            game.Update(gameUpdatedEvent.Title, gameUpdatedEvent.Price, gameUpdatedEvent.LaunchYear, gameUpdatedEvent.Developer, (EGenre)Enum.Parse(typeof(EGenre), gameUpdatedEvent.Genre, true));
            await repository.UpdateAsync(game, cancellationToken);

            var gameDocument = await databaseSearch.GetDocumentAsync(game.Id);
            gameDocument.Update(game.Title, game.Developer, game.Genre.ToString(), game.LaunchYear, game.Price);

            await databaseSearch.UpdateDocumentAsync(gameDocument);
        }
    }
}
