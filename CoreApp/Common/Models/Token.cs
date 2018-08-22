using System;

namespace Common.Models
{
    public class Token : BaseModel
    {
        public string Username { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }
    }
}