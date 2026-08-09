using MongoDB.Driver.Core.Configuration;

namespace Play.Common.Settings;

public class MongoDbSettings
{
    private string _connectionString = string.Empty;
    
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string ConnectionString
    {
        get => string.IsNullOrWhiteSpace(_connectionString)
            ? $"mongodb://{Host}:{Port}" 
            : _connectionString;
        init => _connectionString = value;
    } 
}