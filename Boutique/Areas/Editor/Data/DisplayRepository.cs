using System.Linq.Expressions;
using Boutique.Data;
using Boutique.EFRepository;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Areas.Editor.Data;
public class DisplayRepository<TEntity> : IDisplayRepository<TEntity> where TEntity : class
{
    private readonly DisplayHtmlDbContext _context;
    private readonly DbSet<TEntity> _entities;

    public DisplayRepository(DisplayHtmlDbContext context)
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
