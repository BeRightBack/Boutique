
using System.Collections.Generic;
using Boutique.Entity;
using System;

namespace Boutique.Services;
public interface ICategoryService
{
    IList<Category> GetAllCategories();

    IList<Category> GetAllCategoriesWithoutParent();

    Category GetCategoryById(Guid id);

    Category GetCategoryBySeo(string seo);

    void InsertCategory(Category category);

    void UpdateCategory(Category category);

    void DeleteCategories(IList<Guid> ids);

    void InsertProductCategoryMappings(IList<ProductCategoryMapping> productCategoryMappings);

    void DeleteAllProductCategoryMappingsByProductId(Guid productId);
}
