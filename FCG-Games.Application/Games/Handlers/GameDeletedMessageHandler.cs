using FCG.Shared.EventService.Consumer;
using FCG.Shared.EventService.Contracts.Game;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Domain.Games.SearchDocuments;
using System.Text.Json;

namespace FCG_Games.Application.Games.Handlers
{
    public class GameDeletedMessageHandler(
        IGameRepository repository,
        IDatabaseSearch<GameDocument> databaseSearch) : IMessageHandler
    {
        public string MessageType => "GameDeleted";

        public async Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            GameDeletedEvent gameUpdatedEvent = JsonSerializer.Deserialize<GameDeletedEvent>(message)!;
            await repository.DeleteAsync(gameUpdatedEvent.GameId, cancellationToken);
            await databaseSearch.DeleteDocumentAsync(gameUpdatedEvent.GameId);
        }
    }
}
