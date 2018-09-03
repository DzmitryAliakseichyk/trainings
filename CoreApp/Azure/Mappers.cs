using System.Collections.Generic;
using System.Linq;
using Business.Models.KeyPhrases;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

namespace Azure
{
    internal static class Mappers
    {
        public static MultiLanguageBatchInput Map(this KeyPhrasesRequestModel model)
        {
            var retModel = new MultiLanguageBatchInput
            {
                Documents = new List<MultiLanguageInput>()
            };

            foreach (var doc in model.Documents)
            {
                retModel.Documents.Add(new MultiLanguageInput
                {
                    Id = doc.Id,
                    Language = doc.Language,
                    Text = doc.Text
                });
            }

            return retModel;
        }

        public static KeyPhrasesResponseModel Map(this KeyPhraseBatchResult model)
        {
            var retModel = new KeyPhrasesResponseModel();
            if (model.Documents.Any())
            {
                foreach (var doc in model.Documents)
                {
                    retModel.Documents.Add(new DocumentKeyPhrases
                    {
                        Id = doc.Id,
                        KeyPhrases = doc.KeyPhrases.ToList()
                    });
                }
            }

            if (model.Errors.Any())
            {
                foreach (var error in model.Errors)
                {
                    retModel.Errors.Add(new KeyPhrasesErrorModel
                    {
                        Id = error.Id,
                        Message = error.Message
                    });
                }
            }

            return retModel;
        }
    }
}
