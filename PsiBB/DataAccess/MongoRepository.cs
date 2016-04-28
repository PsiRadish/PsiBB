// Obsolete

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
        
        // Remove
        public async Task<bool> Remove(string id)
        {
            ObjectId oId = new ObjectId(id);
            
            var query = Builders<TModel>.Filter.Eq(e => e.Id, oId);
            DeleteResult result = await _collection.DeleteOneAsync(query);
            
            System.Diagnostics.Debug.Print(result.ToJson());
            
            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        /// <summary>
        /// Adds an item to a list of embedded documents.
        /// Example: <c>bool success = await repo.Add("56cd35159d2feb7ef4100e59", e => e.Prescriptions, "cowbell");</c>
        /// </summary>
        /// <typeparam name="TItem">The type of the list item.</typeparam>
        /// <param name="id">Unique id of record containing the list field.</param>
        /// <param name="listField">LINQ/lambda expression indicating the field to be added to.</param>
        /// <param name="itemValue">List item object to add.</param>
        /// <returns>Task that returns whether operation was successful (boolean).</returns>
        /// <example><code>bool success = await repo.Add("56cd35159d2feb7ef4100e59", e => e.Prescriptions, "cowbell");</code></example>
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

        /// <summary>
        /// Full replacement of model in database
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Task that returns whether operation was successful (boolean).</returns>
        public async Task<bool> Replace(TModel model)
        {
            var query = Builders<TModel>.Filter.Eq(e => e.Id, model.Id);
            
            model.DateModified = DateTime.Now; // update modified time
            
            ReplaceOneResult result = await _collection.ReplaceOneAsync(query, model);
            System.Diagnostics.Debug.Print(result.ToJson());
            
            return result.IsAcknowledged && ((!result.IsModifiedCountAvailable && result.MatchedCount == 1) || result.ModifiedCount == 1);
        }
    }
}
