using Boutique.EFRepository;
using Boutique.Entity;
using System;
using System.Linq;
using System.Collections.Generic;
using Boutique.Data;
using Boutique.Areas.Editor.Data;

namespace Boutique.Areas.Editor.Services;
public class DisplayService : IDisplayService
{

    private readonly IDisplayRepository<Content> _repository;

    public DisplayService(
        IDisplayRepository<Content> repository)
    {
        _repository = repository;        
    }

    public IList<Content> GetAllContents()
    {
        var entities = _repository.GetAll()
            .OrderBy(x => x.Name)
            .ToList();

        return entities;
    }
    
    public Content GetContentById(int id)
    {
        if (id == 0)
            return null;

        return _repository.FindByExpression(x => x.Id == id);
    }

    public void InsertContent(Content content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        _repository.Insert(content);
        _repository.SaveChanges();
    }

    public void UpdateContent(Content content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        _repository.Update(content);
        _repository.SaveChanges();
    }

    public void DeleteContents(IList<int> ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        foreach (var id in ids)
            _repository.Delete(GetContentById(id));

        _repository.SaveChanges();
    }

}
