using ApiPDV.Context;
using ApiPDV.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiPDV.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
           
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }


        public T Create(T entity)
        {
            _context.Set<T>().Add(entity);
            //_context.SaveChanges();
            return entity;
        }

        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);
            // _context.SaveChanges(); 
            return entity;
        }

        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            //_context.SaveChanges(); 
            return entity;
        }

        public async Task<PagedList<T>> GetPagedAsync<TKey>(Expression<Func<T, bool>>? predicate, Expression<Func<T, TKey>>? orderPredicate,  int pageNumber, int pageSize)
        {
            var query = _context.Set<T>().AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderPredicate != null)
            {
                query = query.OrderBy(orderPredicate);
            }
            return await PagedList<T>.ToPagedListAsync(query, pageNumber, pageSize);
        }
    }
}
