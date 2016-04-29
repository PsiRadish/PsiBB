/*
  Kyle Fiegener
  Automatic database connection and the gauntlet of parent classes that Mongo.Model inherits from.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
// using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.Entity.Design.PluralizationServices;
using MongoDB.Bson;
// using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace PsiBB.DataAccess.Mongo
{
    public static partial class Mongo
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

        /// <summary>
        /// A Mongo.Element with an ObjectId, for referencing collection-level documents.
        /// </summary>
        public abstract class Document : Element
        {
            internal Document() : base() { } // TODO: Move DataAccess to a separate assembly so this internal actually does something

            public ObjectId Id { get; set; }
        }
    }
}
