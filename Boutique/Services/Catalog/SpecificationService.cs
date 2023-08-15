
using System;
using System.Collections.Generic;
using Boutique.Data;
using Boutique.EFRepository;
using Boutique.Entity;

namespace Boutique.Services;

public class SpecificationService : ISpecificationService
{
    private readonly IRepository<Specification> specificationRepository;
    private readonly IRepository<ProductSpecificationMapping> productSpecificationMappingRepository;

    public SpecificationService(
        IRepository<Specification> specificationRepository,
        IRepository<ProductSpecificationMapping> productSpecificationMappingRepository)
    {
        this.specificationRepository = specificationRepository;
        this.productSpecificationMappingRepository = productSpecificationMappingRepository;
    }

    public IList<Specification> GetAllSpecifications()
    {
        var entities = specificationRepository.GetAll()
            .OrderBy(x => x.Name)
            .ToList();

        return entities;
    }

    public Specification GetSpecificationById(Guid id)
    {
        return specificationRepository.FindByExpression(x => x.Id == id);
    }

    public void InsertSpecification(Specification specification)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        specificationRepository.Insert(specification);
        specificationRepository.SaveChanges();
    }

    public void UpdateSpecification(Specification specification)
    {
        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        specificationRepository.Update(specification);
        specificationRepository.SaveChanges();
    }

    public void DeleteSpecifications(IList<Guid> ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        foreach (var id in ids)
            specificationRepository.Delete(GetSpecificationById(id));

        specificationRepository.SaveChanges();
    }

    public void InsertProductSpecificationMappings(IList<ProductSpecificationMapping> productSpecificationMappings)
    {
        if (productSpecificationMappings == null)
            throw new ArgumentNullException(nameof(productSpecificationMappings));

        foreach (var mapping in productSpecificationMappings)
            productSpecificationMappingRepository.Insert(mapping);

        productSpecificationMappingRepository.SaveChanges();
    }

    public void DeleteAllProductSpecificationMappings(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new ArgumentNullException(nameof(productId));

        var mappings = productSpecificationMappingRepository.FindManyByExpression(x => x.ProductId == productId);

        foreach (var mapping in mappings)
            productSpecificationMappingRepository.Delete(mapping);

        productSpecificationMappingRepository.SaveChanges();
    }
}
