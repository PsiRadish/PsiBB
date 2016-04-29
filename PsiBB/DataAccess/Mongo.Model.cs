/*
  Kyle Fiegener
  Define database operation methods as part of main parent class for collection-level documents
*/

using System;
using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
// using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace PsiBB.DataAccess.Mongo
{
    public static partial class Mongo
    {
        /// <summary>
        /// Parent class for collection-level documents, with static and instance methods for all database operation.
        /// </summary>
        /// <typeparam name="TModel">The type of your inheriting class. Will look a little redundant, but necessary nonetheless.</typeparam>
        public abstract class Model<TModel> : Document, ISupportInitialize where TModel : Document
        {
            private static readonly IMongoCollection<TModel> _collection;

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

                foreach (var propInfo in this.GetType().GetProperties())
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
