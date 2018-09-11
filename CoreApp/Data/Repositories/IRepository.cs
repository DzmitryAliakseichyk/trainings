using System;
using System.Linq.Expressions;
using Common.Models;

namespace Data.Repositories
{
    public interface IRepository<T> where T: BaseModel
    {
        bool CheckIfExist(Expression<Func<T, bool>> filter);
        T Create(T entity);
        T Get(Guid id);
        T Get(Expression<Func<T, bool>> filter = null,
            string includeProperties = "");
        T Update(T entityToUpdate);
        void Delete(Expression<Func<T, bool>> filter);
        void Delete(Guid id);
    }
}