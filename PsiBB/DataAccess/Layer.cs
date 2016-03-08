using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PsiBB.DataAccess
{
    public abstract class Layer
    {
        public static IModelRepository<TModel> GetRepository<TModel>() where TModel : MongoDoc
        {
            return new MongoRepository<TModel>();
        }

        public static IMongoCollection<TModel> GetCollection<TModel>() where TModel : MongoDoc
        {
            return (new MongoRepository<TModel>()).Collection;
        }
    }
}
