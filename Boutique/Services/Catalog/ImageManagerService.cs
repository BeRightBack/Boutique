
using Boutique.EFRepository;
using Boutique.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Boutique.Services;
public class ImageManagerService : IImageManagerService
{
    private readonly IRepository<Image> imageRepository;
    private readonly IRepository<ProductImageMapping> productImagesRepository;

    public ImageManagerService(
        IRepository<Image> imageRepository,
        IRepository<ProductImageMapping> productImagesRepository)
    {
        this.imageRepository = imageRepository;
        this.productImagesRepository = productImagesRepository;
    }

    public IList<Image> GetAllImages()
    {
        var entities = imageRepository.GetAll()
            .OrderBy(x => x.FileName)
            .ToList();

        return entities;
    }

    public Image GetImageById(Guid id)
    {
        return imageRepository.FindByExpression(x => x.Id == id);
    }

    public IList<Image> SearchImages(string keyword)
    {
        return imageRepository.FindManyByExpression(x => x.FileName.Contains(keyword))
            .OrderBy(x => x.FileName)
            .ToList();
    }

    public void InsertImages(List<Image> images)
    {
        if (images == null)
            throw new ArgumentNullException(nameof(images));

        foreach (var image in images)
            imageRepository.Insert(image);

        imageRepository.SaveChanges();
    }

    public void DeleteImages(IList<Guid> ids)
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        foreach (var id in ids)
            imageRepository.Delete(GetImageById(id));

        imageRepository.SaveChanges();
    }

    public void InsertProductImageMappings(IList<ProductImageMapping> productImageMappings)
    {
        if (productImageMappings == null)
            throw new ArgumentNullException(nameof(productImageMappings));

        foreach (var mapping in productImageMappings)
            productImagesRepository.Insert(mapping);

        productImagesRepository.SaveChanges();
    }

    public void DeleteAllProductImageMappings(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new ArgumentNullException(nameof(productId));

        var mappings = productImagesRepository.FindManyByExpression(x => x.ProductId == productId);

        foreach (var mapping in mappings)
            productImagesRepository.Delete(mapping);

        productImagesRepository.SaveChanges();
    }
}