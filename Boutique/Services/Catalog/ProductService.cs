using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using Boutique.EFRepository;
using Boutique.Entity;
using Boutique.Data;

namespace Boutique.Services;
public class ProductService : IProductService
{
    private readonly CatalogDbContext context;
    private readonly IRepository<Product> productRepository;

    public ProductService(
        CatalogDbContext context,
        IRepository<Product> productRepository)
    {
        this.context = context;
        this.productRepository = productRepository;
    }
    public IList<Product> GetAllProducts()
    {
        // TODO: update when lazy loading is available
        var entities = context.Products
            .Include(x => x.Categories).ThenInclude(x => x.Category)
            .Include(x => x.Images).ThenInclude(x => x.Image)
            .Include(x => x.Manufacturers).ThenInclude(x => x.Manufacturer)
            .Include(x => x.Specifications).ThenInclude(x => x.Specification)
            .AsNoTracking()
            .ToList();

        return entities;
    }

    public Product GetProductById(Guid id)
    {
        if (id == Guid.Empty)
            return null;


        // TODO: update when lazy loading is available
        var entity = context.Products
            .Include(x => x.Categories).ThenInclude(x => x.Category)
            .Include(x => x.Images).ThenInclude(x => x.Image)
            .Include(x => x.Manufacturers).ThenInclude(x => x.Manufacturer)
            .Include(x => x.Specifications).ThenInclude(x => x.Specification)
            .AsNoTracking()
            .SingleOrDefault(x => x.Id == id);

        return entity;
    }

    public Product GetProductBySeo(string seo)
    {
        if (seo == "")
            return null;

        // TODO: update when lazy loading is available
        var entity = context.Products
            .Include(x => x.Categories).ThenInclude(x => x.Category)
            .Include(x => x.Images).ThenInclude(x => x.Image)
            .Include(x => x.Manufacturers).ThenInclude(x => x.Manufacturer)
            .Include(x => x.Specifications).ThenInclude(x => x.Specification)
            .AsNoTracking()
            .SingleOrDefault(x => x.SeoUrl == seo);

        return entity;
    }

    public void InsertProduct(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        productRepository.Insert(product);
        productRepository.SaveChanges();
    }

    public void UpdateProduct(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        productRepository.Update(product);
        productRepository.SaveChanges();
    }

    public void DeleteProducts(IList<Guid> ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        foreach (var id in ids)
            productRepository.Delete(GetProductById(id));

        productRepository.SaveChanges();
    }

    public IList<Product> SearchProduct(
        string nameFilter = null,
        string seoFilter = null,
        string[] categoryFilter = null,
        string[] manufacturerFilter = null,
        string[] priceFilter = null,
        bool isPublished = true)
    {
        var result = context.Products
            .Include(x => x.Categories).ThenInclude(x => x.Category)
            .Include(x => x.Images).ThenInclude(x => x.Image)
            .Include(x => x.Manufacturers).ThenInclude(x => x.Manufacturer)
            .Include(x => x.Specifications).ThenInclude(x => x.Specification)
            .AsNoTracking();

        // published filter
        if (isPublished == false)
        {
            result = result.Where(x => x.Published == false);
        }

        // name filter
        if (nameFilter != null && nameFilter.Length > 0)
        {
            result = result.Where(x => x.Name.ToLower().Contains(nameFilter.ToLower()));
        }

        // seo filter
        if (seoFilter != null && seoFilter.Length > 0)
        {
            throw new NotImplementedException();
        }

        // category filter
        if (categoryFilter != null && categoryFilter.Length > 0)
        {
            result = result.Where(x => x
                .Categories.Select(c => c.Category.Name.ToLower())
                .Intersect(categoryFilter.Select(cf => cf.ToLower()))
                .Any()
            );
        }

        // manufacturer filter
        if (manufacturerFilter != null && manufacturerFilter.Length > 0)
        {
            result = result.Where(x => x
                .Manufacturers
                .Select(c => c.Manufacturer.Name.ToLower())
                .Intersect(manufacturerFilter.Select(mf => mf.ToLower()))
                .Any()
            );
        }

        // price filter
        if (priceFilter != null && priceFilter.Length > 0)
        {
            var tmpResult = new List<Product>();
            foreach (var price in priceFilter)
            {
                var p = price.Split('-');
                int minPrice = Int32.Parse(p[0]);
                int maxPrice = Int32.Parse(p[1]);

                var r = result.Where(x => x.RetailPrice >= minPrice && x.RetailPrice <= maxPrice);
                if (r.Any()) tmpResult.AddRange(r);
            }
            result = tmpResult.AsQueryable();
        }

        return result.ToList();
    }

    public IQueryable<Product> Table()
    {
        return context.Products;
    }
}
