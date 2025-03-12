using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.BaseService.Repository
{
    public interface IRepository<TContext> where TContext : DbContext
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void Add(object obj);
        Task AddAsync(object obj);
        void AddRange(List<object> objs);
        Task AddRangeAsync(List<object> objs);
        void Remove(object obj);
        void RemoveRange(List<object> objs);
        object FindById(Type objectType, long id);
        void RefreshContext(object obj);
    }
}
