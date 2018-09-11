using System;
using System.Linq;
using System.Linq.Expressions;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories.MsSql
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseModel 
    {
        protected DbContext Context;
        protected DbSet<TEntity> DbSet;
        protected ILogger<Repository<TEntity>> Logger;

        protected Repository(DbContext context, ILogger<Repository<TEntity>> logger)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
            Logger = logger;
        }

        public bool CheckIfExist(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                return DbSet.Count(filter) > 0;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public TEntity Create(TEntity entity)
        {
            try
            {
                DbSet.Add(entity);
                Context.SaveChanges();
                return entity;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public TEntity Get(Guid id)
        {
            return DbSet.Find(id);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "")
        {
            try
            {
                IQueryable<TEntity> query = DbSet;

                if (filter != null)
                    query = query.Where(filter);

                var properties = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var includeProperty in properties)
                {
                    query = query.Include(includeProperty);
                }

                return query.FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public TEntity Update(TEntity entityToUpdate)
        {
            try
            {
                DbSet.Attach(entityToUpdate);
                Context.Entry(entityToUpdate).State = EntityState.Modified;
                Context.SaveChanges();
                return entityToUpdate;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public void Delete(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                var entityToDelete = Get(filter);
                if (Context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    DbSet.Attach(entityToDelete);
                }

                DbSet.Remove(entityToDelete);
                Context.SaveChanges();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public void Delete(Guid id)
        {
            Delete(x => x.Id == id);
        }
    }
}