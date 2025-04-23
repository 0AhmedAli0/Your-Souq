using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetEntityWithSpec(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        //Task<IReadOnlyList<string>> GetBrandsAsync();
        //Task<IReadOnlyList<string>> GetTypesAsync();
        void Add(T Entity);
        void Update(T Entity);
        void Remove(T Entity);
        bool Exists(int id);
        Task<bool> SaveChangesAsync();
    }
}
