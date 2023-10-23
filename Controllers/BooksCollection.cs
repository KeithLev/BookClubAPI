using BookClubAPI.MongoDB;
using BookClubClassLibrary;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BookClubAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksCollection : Controller
    {
        private readonly IMongoDBConnection _mongoDBConnection;
        private MongoCollectionBase<Book> _booksCollection;
        private IConfiguration _configuration;

        public BooksCollection(IMongoDBConnection mongoDBConnection, IConfiguration configuration)
        {
            _configuration = configuration;
            _mongoDBConnection = mongoDBConnection;
            _booksCollection = _mongoDBConnection.GetMongoCollection<Book>(_configuration.GetValue<String>("BookCollection"), _configuration.GetValue<String>("MongoDBName"));
        }

        [HttpGet]
        public List<Book> Index()
        {
            try
            {
                var filter = FilterDefinition<Book>.Empty;
                List<Book> books = _booksCollection.Find(filter).ToList();
                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public Book GetBook(string id)
        {
            try
            {
                var filter = Builders<Book>.Filter.Eq("id", id);
                Book book = _booksCollection.Find(filter).FirstOrDefault();
                return book;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [HttpPost]
        public async Task<bool> Create(Book book)
        {
            try
            {
                Task task = _booksCollection.InsertOneAsync(book);

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
                var filter = Builders<Book>.Filter.Eq("id", id);
                var result = await _booksCollection.DeleteOneAsync(filter);
                return result.IsAcknowledged;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<bool> Update(string id, Book book)
        {
            try
            {
                var filter = Builders<Book>.Filter.Eq("id", id);
                var update = Builders<Book>.Update.Set("Title", book.BookName).Set("RecomendedBy", book.RecomendedByUserID).Set("Ratings", book.Ratings);
                var result = await _booksCollection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
