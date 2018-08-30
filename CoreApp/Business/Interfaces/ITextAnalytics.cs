using Business.Models.KeyPhrases;

namespace Business.Interfaces
{
    public interface ITextAnalytics
    {
        KeyPhrasesResponseModel GetKeyPhrases(KeyPhrasesRequestModel model);
    }
}
