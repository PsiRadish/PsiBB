using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PsiBB.DataAccess
{
    public class ModelRepository<TModel> where TModel : CollectionModel
    {
        private static readonly IMongoDatabase __database;

        // setup the database connection to be used by all ModelRepository instances
        static ModelRepository()
        {
            string databaseName = ConfigurationManager.AppSettings["databaseName"];
            string connectionString = ConfigurationManager.ConnectionStrings[databaseName].ConnectionString;

            // var client = new MongoClient(connectionString);
            __database = (new MongoClient(connectionString)).GetDatabase(databaseName);
        }

        private readonly IMongoCollection<TModel> _collection;
        
        public ModelRepository()
        {
            string collectionName = (string)typeof(TModel).GetField("CollectionName").GetValue(null); // Static field should indicate model's name in database.

            _collection = __database.GetCollection<TModel>(collectionName);
        }
        
        public void Add(TModel model)
        {
            _collection.InsertOneAsync(model);
        }
        
        public IEnumerable<TModel> GetAll()
        {
            var models = _collection.Find(new BsonDocument()).ToListAsync();
            
            return models.Result;
        }
        
        public TModel GetById(ObjectId id)
        {
            var query = Builders<TModel>.Filter.Eq(e => e.Id, id);
            var model = _collection.Find(query).ToListAsync();
            
            return model.Result.FirstOrDefault();
        }
        
        public bool Remove(ObjectId id)
        {
            var query = Builders<TModel>.Filter.Eq(e => e.Id, id);
            var result = _collection.DeleteOneAsync(query);
            
            return GetById(id) == null;
        }
        public bool Remove(TModel model)
        {
            var id = model.Id;
            var query = Builders<TModel>.Filter.Eq(e => e.Id, id);
            var result = _collection.DeleteOneAsync(query);
            
            return GetById(id) == null;
        }
        
        public void Update(TModel model)
        {
            var query = Builders<TModel>.Filter.Eq(e => e.Id, model.Id);
            var update = _collection.ReplaceOneAsync(query, model);
        }
    }
}
