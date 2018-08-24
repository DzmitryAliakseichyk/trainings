using System;

namespace Common.Models
{
    public abstract class BaseToken : BaseModel
    {
        public Guid UserId { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }
    }
}