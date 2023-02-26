using System.Linq.Expressions;
using MongoDB.Driver;
using Play.Common.Entites;
using Play.Common.Repositories;

namespace Play.Common.MongoDB;

public class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity
{
    private readonly IMongoCollection<TEntity> _dbCollection;
    private readonly FilterDefinitionBuilder<TEntity> _filterBuilder = Builders<TEntity>.Filter;

    public MongoRepository(IMongoDatabase mongoDatabase, string collectionName)
    {
        _dbCollection = mongoDatabase.GetCollection<TEntity>(collectionName);
    }

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync()
    {
        return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
    }

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await _dbCollection.Find(filter).ToListAsync();
    }

    public async Task<TEntity> GetAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(TEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await _dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        await _dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        await _dbCollection.DeleteOneAsync(filter);
    }
}