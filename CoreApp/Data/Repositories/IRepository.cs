using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Models;

namespace Data.Repositories
{
    public interface IRepository<T> where T : BaseModel
    {
        Task<T> Create(T token);
        Task<T> Get(Guid id);
        Task<T> Get(Expression<Func<T, bool>> condition);
        Task<T> Update(T token);
        Task Delete(Guid id);
        Task Delete(Expression<Func<T, bool>> condition);
        Task<bool> CheckIfExist(Expression<Func<T, bool>> condition);
    }
}