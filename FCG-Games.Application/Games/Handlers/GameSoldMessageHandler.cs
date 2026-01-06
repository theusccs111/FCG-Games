using FCG.Shared.EventService.Consumer;
using FCG.Shared.EventService.Contracts.Game;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Domain.Games.SearchDocuments;
using System.Text.Json;

namespace FCG_Games.Application.Games.Handlers
{
    public class GameSoldMessageHandler(
        IDatabaseSearch<GameDocument> databaseSearch) : IMessageHandler
    {
        public string MessageType => "GameSold";

        public async Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            GameSoldEvent gameSoldEvent = JsonSerializer.Deserialize<GameSoldEvent>(message)!;

            var gameDocument = await databaseSearch.GetDocumentAsync(gameSoldEvent.GameId);
            gameDocument.UpdateSalesCount();

            await databaseSearch.UpdateDocumentAsync(gameDocument);
        }
    }
}
