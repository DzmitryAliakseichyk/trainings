using System.Collections.Generic;

namespace Business.Models.KeyPhrases
{
    public class DocumentKeyPhrases
    {
        public string Id { get; set; }

        public  List<string> KeyPhrases { get; set; }
    }
}