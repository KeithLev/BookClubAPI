using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
namespace BookClubAPI.MongoDB
{
    public class MongoDBConnection : IMongoDBConnection
    {
        private IConfiguration _configuration;

        public MongoDBConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IMongoDatabase GetMongoDBConnection(string db)
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString(_configuration.GetConnectionString("MongoDB"));
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                var client = new MongoClient(settings);
                var database = client.GetDatabase(db);
                return database;
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting to MongoDB", ex);
            }
        }

        public MongoCollectionBase<T> GetMongoCollection<T>(string collectionName, string db)
        {
            try
            {
                var database = GetMongoDBConnection(db);
                var collection = database.GetCollection<T>(collectionName);
                return (MongoCollectionBase<T>)collection;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting MongoDB collection", ex);
            }
        }

    }
}
