using System.Threading.Tasks;
using Business.Models.KeyPhrases;

namespace Business.Interfaces
{
    public interface ITextAnalyticsService
    {
        Task<KeyPhrasesResponseModel> GetKeyPhrasesAsync(KeyPhrasesRequestModel model);
    }
}
