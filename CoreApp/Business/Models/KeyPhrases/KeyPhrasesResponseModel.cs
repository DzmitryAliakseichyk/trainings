using System.Collections.Generic;

namespace Business.Models.KeyPhrases
{
    public class KeyPhrasesResponseModel
    {
        public List<DocumentKeyPhrases> Documents { get; set; }

        public List<KeyPhrasesErrorModel> Errors { get; set; }
    }
}