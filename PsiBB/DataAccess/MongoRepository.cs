using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Configuration;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.Entity.Design.PluralizationServices;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace PsiBB.DataAccess
{
    public class MongoRepository<TModel> : IModelRepository<TModel> where TModel : MongoDoc
    {
        private static readonly IMongoDatabase __database;
        
        // private static Dictionary<> _collections;
        
        // Sets up the database connection to be used by all MongoRepository instances
        static MongoRepository()
        {
            string databaseName = ConfigurationManager.AppSettings["databaseName"];
            string connectionString = ConfigurationManager.ConnectionStrings[databaseName].ConnectionString;
            
            /*// class map alternative to BsonElement attributes in MongoDoc definition
            BsonClassMap.RegisterClassMap<MongoDoc>(cm =>
            {
                cm.AutoMap();
                // cm.MapMember(c => c._dateCreated).SetElementName("DateCreated");
                cm.MapField("_dateCreated").SetElementName("DateCreated");
                // cm.MapMember(c => c._dateModified).SetElementName("DateModified");
                cm.MapField("_dateModified").SetElementName("DateModified");
            });*/
            
            __database = (new MongoClient(connectionString)).GetDatabase(databaseName);
        }
        
        private readonly IMongoCollection<TModel> _collection;
        public IMongoCollection<TModel> Collection
        {
            get
            {
                return _collection;
            }
        }
        
        public MongoRepository()
        {
            // pluralize class name to get MongoDB collection name
            string collectionName = PluralizationService.CreateService(new CultureInfo("en-US")).Pluralize(typeof(TModel).Name); // TODO: move "en-US" to config with a name like "pluralCulture"
            
            _collection = __database.GetCollection<TModel>(collectionName);
        }
        
        // public void Create(TModel model)
        public async Task Create(TModel model)
        {
            DateTime now = DateTime.Now;
            model.DateCreated = now;
            model.DateModified = now;
            
            try
            {
                await _collection.InsertOneAsync(model);
            }
            catch (Exception e)
            {
                // System.Diagnostics.Debug.Print(e.ToJson());
                System.Diagnostics.Debug.Print(e.ToString());
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
            
            var query = Builders<TModel>.Filter.Eq(e => e.Id, oId);
            /*var model = _collection.Find(query).ToListAsync();
            
            return model.Result.FirstOrDefault();*/
            // return _collection.Find(query).FirstOrDefaultAsync().Result;
            
            return await _collection.Find(query).FirstOrDefaultAsync();
            // return await _collection.Find(doc => doc.Id == id).FirstOrDefaultAsync();
        }
        
        // public bool Remove(TModel model)
        public async Task<bool> Remove(string id)
        {
            ObjectId oId = new ObjectId(id);
            
            var query = Builders<TModel>.Filter.Eq(e => e.Id, oId);
            DeleteResult result = await _collection.DeleteOneAsync(query);
            
            System.Diagnostics.Debug.Print(result.ToJson());
            
            return result.IsAcknowledged && result.DeletedCount == 1;
        }
        
        // Add
        public async Task<bool> Add<TItem>(string id, Expression<Func<TModel, IEnumerable<TItem>>> listField, TItem itemValue) where TItem : IHasTimestamps
        {
            ObjectId oId = new ObjectId(id);
            
            var query = Builders<TModel>.Filter.Eq(e => e.Id, oId);
            var update = Builders<TModel>.Update.Push(listField, itemValue);
            
            DateTime now = DateTime.Now;
            itemValue.DateCreated = now;
            itemValue.DateModified = now;
            
            UpdateResult result = await _collection.UpdateOneAsync(query, update);
            System.Diagnostics.Debug.Print(result.ToJson());
            
            return result.IsAcknowledged && ((!result.IsModifiedCountAvailable && result.MatchedCount == 1) || result.ModifiedCount == 1);
        }
        // Full replacement of model in database
        public async Task<bool> FullUpdate(TModel model)
        {
            var query = Builders<TModel>.Filter.Eq(e => e.Id, model.Id);
            
            model.DateModified = DateTime.Now; // update modified time
            
            ReplaceOneResult result = await _collection.ReplaceOneAsync(query, model);
            System.Diagnostics.Debug.Print(result.ToJson());
            
            return result.IsAcknowledged && ((!result.IsModifiedCountAvailable && result.MatchedCount == 1) || result.ModifiedCount == 1);
        }
    }
}
