using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookClubClassLibrary;
using BookClubAPI.MongoDB;
using MongoDB.Driver;
using Microsoft.OpenApi.Writers;

namespace BookClubAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersCollection : Controller
    {
        private readonly IMongoDBConnection _mongoDBConnection;
        private MongoCollectionBase<User> _usersCollection;
        private IConfiguration _configuration;
        public UsersCollection(IMongoDBConnection mongoDBConnection, IConfiguration configuration)
        {
            _configuration = configuration;
            _mongoDBConnection = mongoDBConnection;
            _usersCollection = _mongoDBConnection.GetMongoCollection<User>(_configuration.GetValue<String>("UserCollection"), _configuration.GetValue<String>("MongoDBName"));

        }

        // GET: Users
        [HttpGet]
        public List<User> Index()
        {
            try
            {
                var filter = FilterDefinition<User>.Empty;
                List<User> users = _usersCollection.Find(filter).ToList();
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public User GetUser(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("id", id);
                User user = _usersCollection.Find(filter).FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // GET: Users/Create
        [HttpPost]
        public async Task<bool> Create(User user)
        {
            try
            {
                Task task = _usersCollection.InsertOneAsync(user);

                task.Wait();

                return task.IsCompletedSuccessfully;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<bool> Delete(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("id", id);
                var result = await _usersCollection.DeleteOneAsync(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<bool> Update(string id, User user)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("id", id);
                var update = Builders<User>.Update.Set("Name", user.Name);
                var result = await _usersCollection.UpdateOneAsync(filter, update);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
