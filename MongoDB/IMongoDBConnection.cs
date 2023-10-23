using MongoDB.Driver;

namespace BookClubAPI.MongoDB
{
    public interface IMongoDBConnection
    {
        MongoCollectionBase<T> GetMongoCollection<T>(string collectionName, string db);
    }
}
