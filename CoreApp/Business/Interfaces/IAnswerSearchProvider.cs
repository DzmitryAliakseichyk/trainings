using Business.Models;

namespace Business.Interfaces
{
    public interface IAnswerSearchProvider
    {
        void AddAnswer(QuestionAnswerModel questionAnswerModel);
    }
}
