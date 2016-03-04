using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PsiBB.DataAccess
{
    public abstract class Layer
    {
        public static IModelRepository<TModel> GetRepository<TModel>() where TModel : MongoModel
        {
            return new MongoRepository<TModel>();
        }
    }
}
