using System;
using System.Linq.Expressions;
using Common.Models;

namespace Data.Repositories
{
    public interface IRepository<T> where T : BaseModel
    {
        bool CheckIfExist(Expression<Func<T, bool>> condition);
        T Create(T token);
        T Get(Guid id);
        T Get(Expression<Func<T, bool>> condition);
        T Update(T token);
        void Delete(Expression<Func<T, bool>> condition);
        void Delete(Guid id);
    }
}