
using Boutique.Entity;
using System;
using System.Linq;
using System.Collections.Generic;


namespace Boutique.Services;

public interface IProductService
{
    IList<Product> GetAllProducts();
    Product GetProductById(Guid id);
    Product GetProductBySeo(string seo);
    void InsertProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProducts(IList<Guid> ids);
    IList<Product> SearchProduct(
        string nameFilter = null,
        string seoFilter = null,
        string[] categoryFilter = null,
        string[] manufacturerFilter = null,
        string[] priceFilter = null,
        bool isPublished = true);
    IQueryable<Product> Table();
}
