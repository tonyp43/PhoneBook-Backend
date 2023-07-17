using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PhoneBook_Backend.Data;
using PhoneBook_Backend.Repository.IRepository;

namespace PhoneBook_Backend.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    public readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        this.dbSet = db.Set<T>();
    }

    //Category,CoverType
    public IEnumerable<T> GetAll(string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return query.ToList();
    }

    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return query.FirstOrDefault();
    }

    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public void Delete(T entity)
    {
        dbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entity)
    {
        dbSet.RemoveRange(entity);
    }
}