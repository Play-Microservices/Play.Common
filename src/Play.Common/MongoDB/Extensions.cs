using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Entites;
using Play.Common.Repositories;
using Play.Common.Settings;

namespace Play.Common.MongoDB;

public static class Extensions
{
    public static WebApplicationBuilder AddMongo(this WebApplicationBuilder builder)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        builder.Services.AddSingleton(serviceProvider => 
        {
            var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            var mongoClient = new MongoClient(mongoDbSettings!.ConnectionString);
            return mongoClient.GetDatabase(serviceSettings!.ServiceName);
        });

        return builder;
    }

    public static WebApplicationBuilder AddMongoRepository<TEntity>(this WebApplicationBuilder builder, string collectionName)  
        where TEntity : IEntity
    {
        
        builder.Services.AddSingleton<IRepository<TEntity>, MongoRepository<TEntity>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<TEntity>(database!, collectionName);
        });

        return builder;
    }
}