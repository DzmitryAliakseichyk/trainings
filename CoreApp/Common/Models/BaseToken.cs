using System;

namespace Common.Models
{
    public abstract class BaseToken : BaseModel
    {
        public string Username { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }
    }
}