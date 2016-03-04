using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace PsiBB.DataAccess
{
    // public class ICollectionModel
    public abstract class MongoModel
    {
        public ObjectId Id { get; set; }
    }
}
