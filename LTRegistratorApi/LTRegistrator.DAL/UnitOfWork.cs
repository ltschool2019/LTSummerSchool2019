using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTRegistrator.DAL.Contracts;
using LTRegistrator.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbContext _context;
        private readonly List<object> _repositories = new List<object>();

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public IBaseRepository<T> GetRepository<T>() where T : class
        {
            var repo = (IBaseRepository<T>)_repositories.SingleOrDefault(r => r is IBaseRepository<T>);
            if (repo == null)
            {
                _repositories.Add(repo = new EntityRepository<T>(_context));
            }
            return repo;
        }

        public Task<int> CommitAsync()
        {
            return _context.SaveChangesAsync();
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
            }
            _disposed = true;
        }

        #endregion
    }
}
