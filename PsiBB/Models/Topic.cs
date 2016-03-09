using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace PsiBB.Models
{
    [BsonIgnoreExtraElements]
    public class Topic : DataAccess.MongoModel<Topic>
    {
        [Required]
        public ObjectId AuthorId { get; set; }
        [BsonIgnore]
        public User Author { get; set; } // TODO: Make this automatically populate with data from the User document indicated in AuthorId.
                                         // Could do it with custom setter on AuthorId property, but wouldn't be very DRY for all the other
                                         // places I'd like to do the same thing. Try finding/creating some helpful attributes.
        [Required]
        public ObjectId SectionId { get; set; }

        [Required]
        public string Title { get; set; }

        public class Post : DataAccess.MongoListElement
        {
            [Required]
            public ObjectId AuthorId { get; set; }
            [BsonIgnore]
            public User Author { get; set; } // TODO: "
        }
        public IList<Post> Posts { get; set; }

        //public static async Task<bool> 
    }
}
