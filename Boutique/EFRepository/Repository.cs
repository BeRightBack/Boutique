using System.Linq.Expressions;
using Boutique.Data;
using Microsoft.EntityFrameworkCore;

namespace Boutique.EFRepository;
#pragma warning disable CS8603 // Possible null reference return.
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly CatalogDbContext _context;
    private readonly DbSet<TEntity> _entities;

    public Repository(CatalogDbContext context)
    {
        _context = context;
        _entities = context.Set<TEntity>();
    }

    public IQueryable<TEntity> GetAll()
    {
        return _entities.AsNoTracking();
    }

    public TEntity FindByExpression(Expression<Func<TEntity, bool>> predicate)
    {
        return _entities
            .AsNoTracking()
            .SingleOrDefault(predicate);
    }

    public IQueryable<TEntity> FindManyByExpression(Expression<Func<TEntity, bool>> predicate)
    {
        return _entities
            .AsNoTracking()
            .Where(predicate);
    }

    public TEntity FindById(Guid id)
    {
        throw new NotImplementedException();
    }

    public void Insert(TEntity entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entities.Add(entity);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Update(TEntity entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Delete(TEntity entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entities.Remove(entity);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
