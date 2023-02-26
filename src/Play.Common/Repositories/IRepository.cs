using System.Linq.Expressions;
using Play.Common.Entites;

namespace Play.Common.Repositories;

public interface IRepository<TEntity> where TEntity : IEntity
{
    Task CreateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
    Task<IReadOnlyCollection<TEntity>> GetAllAsync();
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter);
    Task<TEntity> GetAsync(Guid id);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter);
    Task UpdateAsync(TEntity entity);
}