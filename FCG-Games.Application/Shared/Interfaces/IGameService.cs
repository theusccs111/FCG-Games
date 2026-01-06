using FCG_Games.Application.Games.Requests;
using FCG_Games.Application.Games.Responses;
using FCG_Games.Application.Shared.Results;

namespace FCG_Games.Application.Shared.Interfaces
{
    public interface IGameService
    {
        Task<Result> CreateGameAsync(GameRequest request, CancellationToken cancellationToken = default);
        Task<Result<GameResponse>> GetGameByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<IEnumerable<GameResponse>>> GetAllGamesAsync(int page, CancellationToken cancellationToken = default);
        Task<PagedResult<IEnumerable<GameResponse>>> GetCustomizedGamesSearchAsync(int page, Guid userId, string search, CancellationToken cancellationToken = default);
        Task<PagedResult<IEnumerable<GameResponse>>> GetBestSellingGamesAsync(int page, CancellationToken cancellationToken = default);
        Task<Result> DeleteGameAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> UpdateGameAsync(Guid id, GameRequest request, CancellationToken cancellationToken = default);
    }
}
