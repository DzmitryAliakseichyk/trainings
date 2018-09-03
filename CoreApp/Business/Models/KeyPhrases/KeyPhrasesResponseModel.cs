using System.Collections.Generic;

namespace Business.Models.KeyPhrases
{
    public class KeyPhrasesResponseModel
    {
        public List<DocumentKeyPhrases> Documents { get; set; } = new List<DocumentKeyPhrases>();

        public List<KeyPhrasesErrorModel> Errors { get; set; } = new List<KeyPhrasesErrorModel>();
    }
}