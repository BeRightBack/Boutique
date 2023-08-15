using Boutique.EFRepository;
using Boutique.Entity;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Boutique.Services;
public class ManufacturerService : IManufacturerService
{    
    private readonly IRepository<Manufacturer> manufacturerRepository;
    private readonly IRepository<ProductManufacturerMapping> productManufacturerRepository;

    public ManufacturerService(        
        IRepository<Manufacturer> manufacturerRepository,
        IRepository<ProductManufacturerMapping> productManufacturerRepository)
    {        
        this.manufacturerRepository = manufacturerRepository;
        this.productManufacturerRepository = productManufacturerRepository;
    }

    public IList<Manufacturer> GetAllManufacturers()
    {
        var entities = manufacturerRepository.GetAll()
            .OrderBy(x => x.Name)
            .ToList();

        return entities;
    }

    public Manufacturer GetManufacturerById(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return manufacturerRepository.FindByExpression(x => x.Id == id);
    }

    public Manufacturer GetManufacturerBySeo(string seo)
    {
        if (seo == string.Empty)
            return null;

        return manufacturerRepository.FindByExpression(x => x.SeoUrl == seo);
    }

    public void InsertManufacturer(Manufacturer manufacturer)
    {
        if (manufacturer == null)
            throw new ArgumentNullException(nameof(manufacturer));

        manufacturerRepository.Insert(manufacturer);
        manufacturerRepository.SaveChanges();
    }

    public void UpdateManufacturer(Manufacturer manufacturer)
    {
        if (manufacturer == null)
            throw new ArgumentNullException(nameof(manufacturer));

        manufacturerRepository.Update(manufacturer);
        manufacturerRepository.SaveChanges();
    }

    public void DeleteManufacturers(IList<Guid> ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        foreach (var id in ids)
            manufacturerRepository.Delete(GetManufacturerById(id));

        manufacturerRepository.SaveChanges();
    }

    public void InsertProductManufacturerMappings(IList<ProductManufacturerMapping> productManufacturerMappings)
    {
        if (productManufacturerMappings == null)
            throw new ArgumentNullException(nameof(productManufacturerMappings));

        foreach (var mapping in productManufacturerMappings)
            productManufacturerRepository.Insert(mapping);

        productManufacturerRepository.SaveChanges();
    }

    public void DeleteAllProductManufacturersMappings(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new ArgumentNullException(nameof(productId));

        var mappings = productManufacturerRepository.FindManyByExpression(x => x.ProductId == productId);

        foreach (var mapping in mappings)
            productManufacturerRepository.Delete(mapping);

        productManufacturerRepository.SaveChanges();
    }
}