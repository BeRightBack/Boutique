
using Boutique.Entity;
using System;
using System.Collections.Generic;

namespace Boutique.Services;

public interface IManufacturerService
{
    IList<Manufacturer> GetAllManufacturers();

    Manufacturer GetManufacturerById(Guid id);

    Manufacturer GetManufacturerBySeo(string seo);

    void InsertManufacturer(Manufacturer manufacturer);

    void UpdateManufacturer(Manufacturer manufacturer);

    void DeleteManufacturers(IList<Guid> ids);

    void InsertProductManufacturerMappings(IList<ProductManufacturerMapping> productManufacturerMappings);

    void DeleteAllProductManufacturersMappings(Guid productId);
}
