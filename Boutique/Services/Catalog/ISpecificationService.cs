
using System;
using System.Collections.Generic;
using Boutique.Entity;

namespace Boutique.Services;

public interface ISpecificationService
{
    IList<Specification> GetAllSpecifications();
    Specification GetSpecificationById(Guid id);
    void InsertSpecification(Specification specification);
    void UpdateSpecification(Specification specification);
    void DeleteSpecifications(IList<Guid> ids);
    void InsertProductSpecificationMappings(IList<ProductSpecificationMapping> productSpecificationMappings);
    void DeleteAllProductSpecificationMappings(Guid productId);
}
