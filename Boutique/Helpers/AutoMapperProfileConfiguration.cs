﻿using AutoMapper;
using Boutique.Entity;
using Boutique.Models;
using Boutique.Areas.Admin.Models;
using Boutique.Areas.Editor.Models;

namespace Boutique.Helpers;

public class AutoMapperProfileConfiguration : Profile
{
    public AutoMapperProfileConfiguration()
    {
        // billing address mappings
        CreateMap<BillingAddress, BillingAddressModel>()
            .ReverseMap();
        CreateMap<BillingAddress, CheckoutViewModel>()
            .ReverseMap();

        // category mappings
        CreateMap<Category, CategoryListModel>();
        CreateMap<Category, CategoryCreateOrUpdateModel>()
            .ReverseMap();

        // manufacturer mappings
        CreateMap<Manufacturer, ManufacturerListModel>();
        CreateMap<Manufacturer, ManufacturerCreateOrUpdateModel>()
            .ReverseMap();

        // order mapping
        CreateMap<OrderManageModel, Order>();

        // product mappings
        CreateMap<Product, ProductListModel>();
        CreateMap<Product, ProductDetailsModel>()
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Specifications, opt => opt.Ignore());
        CreateMap<Product, ProductViewModel>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Manufacturers, opt => opt.Ignore())
            .ForMember(dest => dest.Specifications, opt => opt.Ignore());
        CreateMap<Product, ProductCreateOrUpdateModel>()
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Specifications, opt => opt.Ignore());
        CreateMap<ProductCreateOrUpdateModel, Product>()
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Specifications, opt => opt.Ignore());

        CreateMap<Review, ReviewViewModel>()
            .ReverseMap();

        // specifications
        CreateMap<Specification, SpecificationListModel>();
        CreateMap<Specification, SpecificationCreateOrUpdateModel>()
            .ReverseMap();

        // suport
        CreateMap<ContactUsMessage, ContactUsMessageModel>();

        CreateMap<Content, ContentViewModel>().ReverseMap();
    }
}
