using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace framework.BaseService.Repository
{
    public class RepositoryImplementor<TContext> : IRepository<TContext> where TContext : DbContext
    {
        #region Consturctor
        private readonly TContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RepositoryImplementor(TContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        public int SaveChanges()
        {
            ApplyAuditFields();
            return _context.SaveChanges();
        }
        public async Task<int> SaveChangesAsync()
        {
            ApplyAuditFields();
            return await _context.SaveChangesAsync();
        }
        public void Add(object obj) => _context.Add(obj);
        public async Task AddAsync(object obj) => await _context.AddAsync(obj);
        public void AddRange(List<object> objs) => _context.AddRange(objs);
        public async Task AddRangeAsync(List<object> objs) => await _context.AddRangeAsync(objs);
        public void Remove(object obj) => _context.Remove(obj);
        public void RemoveRange(List<object> objs) => _context.RemoveRange(objs);
        public object FindById(Type objectType, long id) => _context.Find(objectType, id)!;
        public void RefreshContext(object obj) => _context.Entry(obj).State = EntityState.Detached;

        private void ApplyAuditFields()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var entities = _context.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var entity in entities)
            {
                var entityType = entity.Entity.GetType();
                var usrCrtProperty = entityType.GetProperty("UsrCrt");
                var usrUpdProperty = entityType.GetProperty("UsrUpd");
                var dtmCrtProperty = entityType.GetProperty("DtmCrt");
                var dtmUpdProperty = entityType.GetProperty("DtmUpd");

                if (entity.State == EntityState.Added)
                {
                    usrCrtProperty?.SetValue(entity.Entity, userName);
                    dtmCrtProperty?.SetValue(entity.Entity, DateTime.Now);
                }

                usrUpdProperty?.SetValue(entity.Entity, userName);
                dtmUpdProperty?.SetValue(entity.Entity, DateTime.Now);
            }
        }
    }
}
