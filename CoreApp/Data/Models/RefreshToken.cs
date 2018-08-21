using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }
    }
}