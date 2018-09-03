using System.Collections.Generic;

namespace Business.Models.KeyPhrases
{
    public class KeyPhrasesRequestModel
    {
        public List<KeyPhrasesRequestDocument> Documents { get; set; } = new List<KeyPhrasesRequestDocument>();
    }
}