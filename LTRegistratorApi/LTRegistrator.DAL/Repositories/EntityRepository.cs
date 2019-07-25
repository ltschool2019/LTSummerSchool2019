using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LTRegistrator.DAL.Contracts;
using LTRegistrator.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.DAL.Repositories
{
    public class EntityRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<T> DbSet;

        public EntityRepository(DbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public virtual IQueryable<T> All()
        {
            return DbSet;
        }

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = All();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public virtual T FindById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual async Task<T> FindByIdAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual IQueryable<T> Where(params Expression<Func<T, bool>>[] predicates)
        {
            IQueryable<T> query = All();
            foreach (var predicate in predicates)
            {
                query = query.Where(predicate);
            }
            return query;
        }

        public virtual bool Any(Expression<Func<T, bool>> predicates)
        {
            return All().Any(predicates);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicates)
        {
            return await All().AnyAsync(predicates);
        }

        public T SingleOrDefault()
        {
            return All().SingleOrDefault();
        }

        public async Task<T> SingleOrDefaultAsync()
        {
            return await All().SingleOrDefaultAsync();
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicates)
        {
            return All().SingleOrDefault(predicates);
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicates)
        {
            return await All().SingleOrDefaultAsync(predicates);
        }

        public virtual T Single()
        {
            return All().Single();
        }

        public T Single(Expression<Func<T, bool>> where)
        {
            return All().Single(where);
        }

        public T FirstOrDefault()
        {
            return All().FirstOrDefault();
        }
        public async Task<T> FirstOrDefaultAsync()
        {
            return await All().FirstOrDefaultAsync();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicates)
        {
            return All().FirstOrDefault(predicates);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicates)
        {
            return await All().FirstOrDefaultAsync(predicates);
        }

        public virtual T First()
        {
            return All().First();
        }

        public T First(Expression<Func<T, bool>> where)
        {
            return All().First(where);
        }

        public virtual void Add(T entity)
        {
            if (entity is ICreatedAt createdDate)
            {
                createdDate.CreatedAt = DateTime.UtcNow;
            }

            if (entity is IModifiedAt updatedDate)
            {
                updatedDate.ModifiedAt = DateTime.UtcNow;
            }

            var dbEntityEntry = Context.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }

        public virtual void Update(T entity)
        {
            if (entity is IModifiedAt updatedDate)
            {
                updatedDate.ModifiedAt = DateTime.UtcNow;
            }
            var dbEntityEntry = Context.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Remove(T entity)
        {
            var dbEntityEntry = Context.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public virtual void Remove(object id)
        {
            var entity = FindById(id);
            if (entity == null) return; // not found; assume already deleted.
            Remove(entity);
        }

        public void Remove(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}
