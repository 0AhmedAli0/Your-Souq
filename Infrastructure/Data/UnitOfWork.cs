using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class UnitOfWork(StoreContext context) : IUnitOfWork
    {
        private readonly ConcurrentDictionary<string, object> _repositories = new();
        public async Task<bool> Complete()
        {
           return await context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {//Dispose will work when the unit of work is out of scope => ستقوم بالتخلص من كل محتويات وحده العمل عندما تخرج من النطاق
            context.Dispose();//release the allocated resources for this context.
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;

            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(type, t =>
            {
                var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));
                /*typeof(GenericRepository<>):
                    يحصل على كائن Type الذي يمثل النوع العام GenericRepository<> بدون تحديد معامل النوع
                    علامة <> تشير إلى أن هذا نوع عام غير مغلق

                    .MakeGenericType(typeof(TEntity)):
                    هذه الطريقة تأخذ معامل نوع (type argument) وتنشئ نسخة محددة من النوع العام
                 */

                return Activator.CreateInstance(repositoryType, context) ??
                    throw new InvalidOperationException($"Could not create instance of {repositoryType.Name}");
                /*Activator.CreateInstance()
                    يسمح بإنشاء مثيلات كائنات ديناميكياً في وقت التشغيل

                    2 params وهذا الشكل يأخذ:
                    1-Type: نوع الكائن المراد إنشاؤه
                    2-params object[]: معاملات المُنشئ
                 */



                //return new GenericRepository<TEntity>(context); السطر ده بديل الكلام الي فوق ده
            });
        }
    }
}
