using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace PsiBB.Models
{
    [BsonIgnoreExtraElements]
    public class User : DataAccess.MongoModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public DateTime DateJoined { get; set; }
    }
}
