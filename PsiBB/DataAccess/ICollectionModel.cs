using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PsiBB.DataAccess
{
    // public class ICollectionModel
    public interface ICollectionModel
    {
        // public virtual string CollectionName
        // {
        //     get
        //     {
        //         // TODO: throw an exception for failing to override this property accessor

        //         return "CollectionModel";
        //     }
        // }

        ObjectId Id { get; set; }
    }
}
