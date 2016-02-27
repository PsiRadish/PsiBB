using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace PsiBB.DataAccess
{
    // public class ICollectionModel
    public abstract class CollectionModel
    {
        // public virtual string CollectionName
        // {
        //     get
        //     {
        //         // TODO: throw an exception for failing to override this property accessor

        //         return "CollectionModel";
        //     }
        // }

        public ObjectId Id { get; set; }
    }
}
