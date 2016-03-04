using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Configuration;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.Entity.Design.PluralizationServices;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PsiBB.DataAccess
{
    public class MongoRepository<TModel> : IModelRepository<TModel> where TModel : MongoModel
    {
        private static readonly IMongoDatabase __database;

        // Sets up the database connection to be used by all MongoRepository instances
        static MongoRepository()
        {
            string databaseName = ConfigurationManager.AppSettings["databaseName"];
            string connectionString = ConfigurationManager.ConnectionStrings[databaseName].ConnectionString;

            // var client = new MongoClient(connectionString);
            __database = (new MongoClient(connectionString)).GetDatabase(databaseName);
        }

        private readonly IMongoCollection<TModel> _collection;
        
        public MongoRepository()
        {
            // string collectionName = (string)typeof(TModel).GetField("CollectionName").GetValue(null); // Static field should indicate model's name in database.
            string collectionName = PluralizationService.CreateService(new CultureInfo("en-US")).Pluralize(typeof(TModel).Name); // TODO: move "en-US" to config with a name like "pluralCulture"

            _collection = __database.GetCollection<TModel>(collectionName);
        }
        
        // public void Create(TModel model)
        public async Task Create(TModel model)
        {
            try
            {
                await _collection.InsertOneAsync(model);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToJson());
            }
        }
        
        // public IEnumerable<TModel> GetAll()
        public async Task<IEnumerable<TModel>> GetAll()
        {
            /*var models = _collection.Find(new BsonDocument()).ToListAsync();
            
            return models.Result;*/
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }
        
        public async Task<TModel> GetById(string id)
        {
            ObjectId oId = new ObjectId(id);
            return await this.GetById(oId);
        }
        // public TModel GetById(ObjectId id)
        public async Task<TModel> GetById(ObjectId id)
        {
            var query = Builders<TModel>.Filter.Eq(e => e.Id, id);
            /*var model = _collection.Find(query).ToListAsync();

            return model.Result.FirstOrDefault();*/
            // return _collection.Find(query).FirstOrDefaultAsync().Result;
            
            return await _collection.Find(query).FirstOrDefaultAsync();
            // return await _collection.Find(doc => doc.Id == id).FirstOrDefaultAsync();
        }
        
        /*public bool Remove(ObjectId id)
        {
            var query = Builders<TModel>.Filter.Eq(e => e.Id, id);
            var result = _collection.DeleteOneAsync(query);
            
            return GetById(id) == null;
        }*/
        // public bool Remove(TModel model)
        public async Task<bool> Remove(string id)
        {
            ObjectId oId = new ObjectId(id);
            var query = Builders<TModel>.Filter.Eq(e => e.Id, oId);
            // var result = _collection.DeleteOneAsync(query).Result;
            
            // return GetById(id) == null;
            
            DeleteResult result = await _collection.DeleteOneAsync(query);
            System.Diagnostics.Debug.Print(result.ToJson());

            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        public async Task<bool> Add<TItem>(string id, Expression<Func<TModel, IEnumerable<TItem>>> listField, TItem itemValue)
        {
            ObjectId oId = new ObjectId(id);
            var query = Builders<TModel>.Filter.Eq(e => e.Id, oId);

            var update = Builders<TModel>.Update.Push(listField, itemValue);

            UpdateResult result = await _collection.UpdateOneAsync(query, update);
            System.Diagnostics.Debug.Print(result.ToJson());

            return result.IsAcknowledged && ((!result.IsModifiedCountAvailable && result.MatchedCount == 1) || result.ModifiedCount == 1);
        }
        // public void FullUpdate(TModel model)
        public async Task<bool> FullUpdate(TModel model)
        {
            var query = Builders<TModel>.Filter.Eq(e => e.Id, model.Id);
            // var result = _collection.ReplaceOneAsync(query, model).Result;
            
            ReplaceOneResult result = await _collection.ReplaceOneAsync(query, model);
            System.Diagnostics.Debug.Print(result.ToJson());

            return result.IsAcknowledged && ((!result.IsModifiedCountAvailable && result.MatchedCount == 1) || result.ModifiedCount == 1);
        }
    }
}
