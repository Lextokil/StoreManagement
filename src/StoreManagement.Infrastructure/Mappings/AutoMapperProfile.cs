using AutoMapper;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Company mappings
        CreateMap<Company, CompanyDto>();
        CreateMap<CreateCompanyDto, Company>();
        CreateMap<UpdateCompanyDto, Company>();
        
        // Store mappings
        CreateMap<Store, StoreDto>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
            .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Company.Code));
        CreateMap<CreateStoreDto, Store>();
        CreateMap<UpdateStoreDto, Store>();
        
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
            .ForMember(dest => dest.StoreCode, opt => opt.MapFrom(src => src.Store.Code));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
    }
}
