using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Domain.Games.Entities;
using FCG_Games.Infrastructure.Shared.Context;
using FCG_Games.Infrastructure.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FCG_Games.Infrastructure.Games.Repositories
{
    public class GameRepository(GamesDbContext context) : GenericRepository<Game>(context), IGameRepository
    {
        private readonly GamesDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<bool> Exists(Game game, CancellationToken cancellationToken = default)
        => await _context.Games
                .AsNoTracking()
                .AnyAsync(g => g.Title == game.Title &&
                               g.Developer == game.Developer &&
                               g.LaunchYear == game.LaunchYear,
                               cancellationToken);

        public async Task<IEnumerable<Game>> FilterListByIds(List<Guid> ids, CancellationToken cancellationToken = default)
            => [.. _context.Games.Where(game => ids.Contains(game.Id))];
    }
}
