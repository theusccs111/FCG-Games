using FCG_Games.Domain.Games.Entities;

namespace FCG_Games.Application.Shared.Interfaces
{
    public interface IGameRepository : IRepository<Game>
    {
        Task<bool> Exists(Game game, CancellationToken cancellationToken = default);
        Task<IEnumerable<Game>> FilterListByIds(List<Guid> ids, CancellationToken cancellationToken = default);
    }
}
