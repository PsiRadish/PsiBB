// Obsolete

using MongoDB.Driver;

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
