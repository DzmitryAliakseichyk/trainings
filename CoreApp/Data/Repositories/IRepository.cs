using System;
using System.Threading.Tasks;
using Common.Models;

namespace Data.Repositories
{
    public interface IRepository<T> where T : BaseModel
    {
        Task<T> Create(T token);
        Task<T> Update(T token);
        Task Delete(Guid id);
        Task Delete(Func<T, bool> condition);
    }
}