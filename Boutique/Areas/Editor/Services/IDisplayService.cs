
using System.Collections.Generic;
using Boutique.Entity;
using System;

namespace Boutique.Areas.Editor.Services;
public interface IDisplayService
{
    IList<Content> GetAllContents();

    Content GetContentById(int id);

    void InsertContent(Content category);

    void UpdateContent(Content category);

    void DeleteContents(IList<int> ids);
}
