using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.Entity.Design.PluralizationServices;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace PsiBB.DataAccess.Mongo
{
    public static class Mongo
    {
        /*
        =======================
           STATIC METHOD(S)
        =======================
        */
        /// <summary>
        /// Generates the MongoDB collection name for models of the passed type.
        /// </summary>
        /// <param name="type">Type of model to generate the collection name for.</param>
        /// <returns>The pluralized name of <code>type</code>.</returns>
        public static string GenerateCollectionName(Type type)
        {
            return PluralizationService.CreateService(new CultureInfo("en-US")).Pluralize(type.Name); // TODO: move "en-US" to config with a name like "pluralCulture"
        }
        
        /*
        ============================================================================
           STATIC MEMBER(S)
        ============================================================================
        */
        private static readonly IMongoDatabase __database;
        
        /*
        ============================================================================
           STATIC CONSTRUCTOR(S)
        ============================================================================
        */
        // Sets up the database connection to be used by all derived instances
        static Mongo()
        {
            string databaseName = ConfigurationManager.AppSettings["databaseName"];
            string connectionString = ConfigurationManager.ConnectionStrings[databaseName].ConnectionString;
            
            __database = (new MongoClient(connectionString)).GetDatabase(databaseName);
        }
        
        /*
        ============================================================================
           NESTED CLASSES
        ============================================================================
        */
        /// <summary>
        /// Class with created and modified dates but no id (good for embedded documents).
        /// </summary>
        public abstract class Element // : Base
        {
            [BsonElement("DateCreated")]
            protected BsonDateTime _dateCreated = new BsonDateTime(0);
            [BsonElement("DateModified")]
            protected BsonDateTime _dateModified = new BsonDateTime(0);
            
            [BsonIgnore]
            public DateTime DateCreated
            {
                get { return _dateCreated.ToUniversalTime(); }
                set { _dateCreated = value; }
            }
            [BsonIgnore]
            public DateTime DateModified
            {
                get { return _dateModified.ToUniversalTime(); }
                set { _dateModified = value; }
            }
        }
        
        /// <summary>
        /// Element with Index property for use in embedded list items.
        /// </summary>
        public abstract class EmbeddedListElement : Element
        {
            // TODO: Populate these fields in Document.EndInit()
            [BsonIgnore]
            public Document Parent { get; set; }    // Parent document
            // public ObjectId ParentId { get; set; }  // Unique id of parent document
            [BsonIgnore]
            public string ListName { get; set; }    // Name of the embedded list
            [BsonIgnore]
            public int Index { get; set; }          // Element's position in the embedded list.
            
            // UPDATE
            // reference: http://stackoverflow.com/questions/29656680/update-an-embedded-document-from-a-collection-using-mongodb-and-c-sharp-new-driv
            //            http://stackoverflow.com/questions/31453681/mongo-update-array-element-net-driver-2-0
            public async Task<bool> UpdateAsync<TParentModel>(string[] fieldNames) where TParentModel : Document
            {
                DateTime now = DateTime.Now;
                this.DateModified = now;
                this.Parent.DateModified = now;
                
                string embeddedFieldDefPrefix = this.ListName + "." + this.Index + ".";
                
                // var parentCollection = __database.GetCollection<TParentModel>(GenerateCollectionName(typeof(TParentModel)));
                var parentCollection = Model<TParentModel>.Collection;
                
                // var query = Builders<TParentModel>.Filter.Eq(e => e.Id, this.ParentId);
                var query = Builders<TParentModel>.Filter.Eq(e => e.Id, this.Parent.Id);
                var updates = new List<UpdateDefinition<TParentModel>>()
                {   // initialize update definition with change to modified date
                    Builders<TParentModel>.Update.Set("DateModified", this._dateModified),
                    Builders<TParentModel>.Update.Set(embeddedFieldDefPrefix + "DateModified", this._dateModified)
                };
                
                // foreach (KeyValuePair<string, dynamic> field in fields)
                foreach (string fieldName in fieldNames)
                {
                    var fieldValue = this.GetType().GetProperty(fieldName).GetValue(this);
                    updates.Add(Builders<TParentModel>.Update.Set(embeddedFieldDefPrefix + fieldName, fieldValue));
                }
                var update = Builders<TParentModel>.Update.Combine(updates);
                
                UpdateResult result = await parentCollection.UpdateOneAsync(query, update);
                System.Diagnostics.Debug.Print(result.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }));
                
                return result.IsAcknowledged && ((!result.IsModifiedCountAvailable && result.MatchedCount == 1) || result.ModifiedCount == 1);
            }
        }
        
        // Element with ObjectId, for collection-level documents
        public abstract class Document : Element
        {
            public ObjectId Id { get; set; }
        }
        
        /// <summary>
        /// Document with supporting database operation methods.
        /// </summary>
        /// <typeparam name="TModel">The type of the inheriting class. Will look a little redundant, but necessary nonetheless.</typeparam>
        public abstract class Model<TModel> : Document, ISupportInitialize where TModel : Document
        {
            // private readonly IMongoCollection<TModel> _collection;
            private static readonly IMongoCollection<TModel> _collection;
            
            // public Model()
            static Model()
            {
                // pluralize class name to get MongoDB collection name
                string collectionName = GenerateCollectionName(typeof(TModel));
                
                _collection = __database.GetCollection<TModel>(collectionName);
            }
            
            /// <summary>
            /// The MongoDB collection for this model.
            /// </summary>
            public static IMongoCollection<TModel> Collection
            {
                get { return _collection; }
            }
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static async Task<TModel> GetByIdAsync(string id)
            {
                ObjectId oId = new ObjectId(id);
                
                var query = Builders<TModel>.Filter.Eq(e => e.Id, oId);
                
                return await _collection.Find(query).FirstOrDefaultAsync();
            }
            
            public static async Task<IEnumerable<TModel>> GetAllAsync()
            {
                return await _collection.Find(new BsonDocument()).ToListAsync();
            }
            
            /// <summary>
            /// Returns this instance with the requisite typecasts applied so it can be passed to the MongoDB driver.
            /// </summary>
            private TModel _T
            {
                get
                {
                    return (TModel)(Document)this;
                }
            }
            
            public async Task CreateAsync()
            {
                DateTime now = DateTime.Now;
                this.DateCreated = now;
                this.DateModified = now;
                
                try
                {
                    await _collection.InsertOneAsync(this._T);
                }
                catch (Exception e)
                {
                    // System.Diagnostics.Debug.Print(e.ToJson(new JsonWriterSettings { Indent = true }));
                    System.Diagnostics.Debug.Print(e.ToString());
                }
            }
            
            /// <summary>
            /// Remove this document from the database.
            /// </summary>
            /// <returns>Whether operation was successful (boolean).</returns>
            public async Task<bool> RemoveAsync()
            {
                var query = Builders<TModel>.Filter.Eq(e => e.Id, this.Id);
                
                DeleteResult result = await _collection.DeleteOneAsync(query);
                System.Diagnostics.Debug.Print(result.ToJson(new JsonWriterSettings { Indent = true }));
                
                return result.IsAcknowledged && result.DeletedCount == 1;
            }
            
            // Add
            public async Task<bool> AddAsync<TItem>(string listFieldName, TItem itemValue) where TItem : EmbeddedListElement    // Expression<Func<TModel, IEnumerable<TItem>>> listFieldName
            {
                IList<TItem> listField = (IList<TItem>)this.GetType().GetProperty(listFieldName).GetValue(this);
                DateTime now = DateTime.Now;
                this.DateModified = now;
                itemValue.DateCreated = now;
                itemValue.DateModified = now;
                
                itemValue.Parent = this;
                itemValue.ListName = listFieldName;
                itemValue.Index = listField.Count;
                
                var query = Builders<TModel>.Filter.Eq(e => e.Id, this.Id);
                var update = Builders<TModel>.Update.Set("DateModified", this._dateModified).Push(listFieldName, itemValue);
                
                UpdateResult result = await _collection.UpdateOneAsync(query, update);
                System.Diagnostics.Debug.Print(result.ToJson(new JsonWriterSettings { Indent = true }));
                
                bool success = result.IsAcknowledged && ((!result.IsModifiedCountAvailable && result.MatchedCount == 1) || result.ModifiedCount == 1);
                if (success)
                {   // also do addition for current deserialized instance
                    listField.Add(itemValue);
                }
                
                return success;
            }
            
            /// <summary>
            /// 
            /// </summary>
            /// <returns>Whether operation was successful (boolean).</returns>
            public async Task<bool> ReplaceAsync()
            {
                this.DateModified = DateTime.Now; // update modified time
                
                var query = Builders<TModel>.Filter.Eq(e => e.Id, this.Id);
                
                ReplaceOneResult result = await _collection.ReplaceOneAsync(query, this._T);
                System.Diagnostics.Debug.Print(result.ToJson(new JsonWriterSettings { Indent = true }));
                
                return result.IsAcknowledged && ((!result.IsModifiedCountAvailable && result.MatchedCount == 1) || result.ModifiedCount == 1);
            }
            
            // These run before and after serialization
            public void BeginInit()
            {
                // System.Diagnostics.Debug.Print("About to deserialize " + this.GetType().Name);
            }
            public void EndInit()
            {
                // System.Diagnostics.Debug.Print(this.GetType().Name + " deserialized!");
                
                foreach(var propInfo in this.GetType().GetProperties())
                {   // search for lists of EmbeddedListElements
                    if (typeof(IEnumerable<EmbeddedListElement>).IsAssignableFrom(propInfo.PropertyType))
                    {   // found one
                        var mongoList = (IList<EmbeddedListElement>)propInfo.GetValue(this);
                        
                        // loop through and set convenience properties
                        for (int i = 0; i < mongoList.Count; i++)  // foreach (var listPair in mongoList.Select((item, index) => new {item, index}))
                        {
                            mongoList[i].Parent = this;
                            mongoList[i].ListName = propInfo.Name;
                            mongoList[i].Index = i; // listPair.index;
                        }
                    }
                }
            }
        }
    }
}
