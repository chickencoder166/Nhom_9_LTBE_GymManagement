using QLPG_a.Models;
using System.Collections.Generic;
using System.Linq;

namespace QLPG_a.Data.Repositories
{
    /// <summary>
    /// Interface cho repository pattern
    /// </summary>
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        bool Exists(int id);
    }
}
