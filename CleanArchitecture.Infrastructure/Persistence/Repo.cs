using CleanArchitecture.Core.Interface;
using CleanArchitecture.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence
{
    public class Repo<TEntity> : IRepo<TEntity> where TEntity : class
    {
        private readonly StoreContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public Repo(StoreContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }


        public async Task DeleteAsync(string id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            // AsNoTracking بتحسن الأداء جداً في الـ Get لأنها بتلغي الـ Tracking بتاع EF
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            // Attach بتحط الكائن في الـ Context
            _dbSet.Attach(entity);
            // السطر ده بيجبر EF إنه يعتبر الكائن "Modified" عشان يولد Update Query صحيحة
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
