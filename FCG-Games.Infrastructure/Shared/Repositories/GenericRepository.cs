using FCG.Shared.Transactional;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Infrastructure.Shared.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG_Games.Infrastructure.Shared.Repositories
{
    public class GenericRepository<T>(GamesDbContext context) : IRepository<T> where T : Entity
    {
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.AddAsync(entity, cancellationToken);
            return context.SaveChangesAsync(cancellationToken);
        }
        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return context.SaveChangesAsync(cancellationToken);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = GetByIdAsync(id, cancellationToken).Result;

            _dbSet.Remove(entity!);
            return context.SaveChangesAsync(cancellationToken);
        }

        public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => _dbSet.AsNoTracking().ToListAsync(cancellationToken).ContinueWith(t => (IEnumerable<T>)t.Result, cancellationToken);

        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            =>  _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);   
    }
}
