﻿using System;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using PsiBB.DataAccess.Mongo;

namespace PsiBB.Models
{
    [BsonIgnoreExtraElements]
    public class User : Mongo.Model<User>
    {
        [Required]
        public string DisplayName { get; set; }
        
        [Required]
        public string Email { get; set; }

        // Just another name for DateCreated property.
        [BsonIgnore]
        public DateTime DateJoined
        {
            get
            {
                return this.DateCreated;
            }
        }
    }
}
