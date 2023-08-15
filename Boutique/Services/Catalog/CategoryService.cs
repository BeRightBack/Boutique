using Boutique.EFRepository;
using Boutique.Entity;
using System;
using System.Linq;
using System.Collections.Generic;
using Boutique.Data;

namespace Boutique.Services;
public class CategoryService : ICategoryService
{
    
    private readonly IRepository<Category> categoryRepository;
    private readonly IRepository<ProductCategoryMapping> productCategoryRepository;

    public CategoryService(        
        IRepository<Category> categoryRepository,
        IRepository<ProductCategoryMapping> productCategoryRepository)
    {
        this.categoryRepository = categoryRepository;
        this.productCategoryRepository = productCategoryRepository;
    }

    public IList<Category> GetAllCategories()
    {
        var entities = categoryRepository.GetAll()
            .OrderBy(x => x.Name)
            .ToList();

        return entities;
    }

    public IList<Category> GetAllCategoriesWithoutParent()
    {
        var entities = categoryRepository.FindManyByExpression(x => x.ParentCategoryId == Guid.Empty)
            .OrderBy(x => x.Name)
            .ToList();

        return entities;
    }

    public Category GetCategoryById(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return categoryRepository.FindByExpression(x => x.Id == id);
    }

    public Category GetCategoryBySeo(string seo)
    {
        if (string.IsNullOrWhiteSpace(seo))
            return null;

        return categoryRepository.FindByExpression(x => x.SeoUrl == seo);
    }

    public void InsertCategory(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        categoryRepository.Insert(category);
        categoryRepository.SaveChanges();
    }

    public void UpdateCategory(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        categoryRepository.Update(category);
        categoryRepository.SaveChanges();
    }

    public void DeleteCategories(IList<Guid> ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        foreach (var id in ids)
            categoryRepository.Delete(GetCategoryById(id));

        categoryRepository.SaveChanges();
    }

    public void InsertProductCategoryMappings(IList<ProductCategoryMapping> productCategoryMappings)
    {
        if (productCategoryMappings == null)
            throw new ArgumentNullException(nameof(productCategoryMappings));

        foreach (var mapping in productCategoryMappings)
            productCategoryRepository.Insert(mapping);

        productCategoryRepository.SaveChanges();
    }

    public void DeleteAllProductCategoryMappingsByProductId(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new ArgumentNullException(nameof(productId));

        var mappings = productCategoryRepository.FindManyByExpression(x => x.ProductId == productId);

        foreach (var mapping in mappings)
            productCategoryRepository.Delete(mapping);

        productCategoryRepository.SaveChanges();
    }
}
