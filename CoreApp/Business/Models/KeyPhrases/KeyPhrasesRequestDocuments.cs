using System;

namespace Business.Models.KeyPhrases
{
    public class KeyPhrasesRequestDocuments
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string Text { get; set; }

        public string Language { get; set; } = "en";
    }
}