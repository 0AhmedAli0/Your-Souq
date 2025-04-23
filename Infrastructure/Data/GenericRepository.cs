using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GenericRepository<T>(StoreContext _context) : IGenericRepository<T> where T : BaseEntity
    {
        public void Add(T Entity)
        {
            _context.Set<T>().Add(Entity);
        }

        public bool Exists(int id)
        {
            return _context.Set<T>().Any(x => x.Id == id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
            //return await SpecificationEvaluator<T>.GetQuery(_context.Set<T>(),spec).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetEntityWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            throw new NotImplementedException();
        }

        public void Remove(T Entity)
        {
            _context.Set<T>().Remove(Entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(T Entity)
        {
            _context.Set<T>().Attach(Entity);//او قادم من واجهه المستخدم context بستخدم الطريقه دي في حاله عندما يكون الكيان غير متصل بالـ
            /*تضيف الكيان إلى Change Tracker الخاص بـ EF Core
            الحالة الابتدائية للكيان تصبح Unchanged (غير معدل)
            EF Core يبدأ بتتبع الكيان ولكن لا يعتبره معدلاً بعد
             */
            _context.Entry(Entity).State = EntityState.Modified;
            /*تغيير حالة الكيان إلى Modified (معدل)
            عند استدعاء SaveChanges()، سيقوم EF Core بإنشاء استعلام UPDATE لتحديث كل خصائص الكيان  
             */
        }

        private IQueryable<T> ApplySpecification (ISpecification<T> spec)
        {
            return  SpecificationEvaluator<T>.GetQuery(_context.Set<T>(),spec);
        }
    }
}
