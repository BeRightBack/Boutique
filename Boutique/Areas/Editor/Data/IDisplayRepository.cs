using System.Linq.Expressions;

namespace Boutique.Areas.Editor.Data;

public interface IDisplayRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetAll();
    TEntity FindByExpression(Expression<Func<TEntity, bool>> predicate);
    void Insert(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    void SaveChanges();
}
