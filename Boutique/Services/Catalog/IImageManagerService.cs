
using Boutique.Entity;
using System;
using System.Collections.Generic;

namespace Boutique.Services;

public interface IImageManagerService
{
    IList<Image> GetAllImages();

    Image GetImageById(Guid id);

    IList<Image> SearchImages(string keyword);

    void InsertImages(List<Image> images);

    void DeleteImages(IList<Guid> ids);

    void InsertProductImageMappings(IList<ProductImageMapping> productImageMappings);

    void DeleteAllProductImageMappings(Guid productId);
}
