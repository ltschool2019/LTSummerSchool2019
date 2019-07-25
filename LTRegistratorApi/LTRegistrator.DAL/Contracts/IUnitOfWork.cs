using System.Threading.Tasks;

namespace LTRegistrator.DAL.Contracts
{
    public interface IUnitOfWork
    {
        IBaseRepository<T> GetRepository<T>() where T : class;
        Task<int> CommitAsync();
        int Commit();
    }
}