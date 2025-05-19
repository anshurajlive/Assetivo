using AutoMapper;

public class PropertyProfile : Profile
{
    public PropertyProfile()
    {
        CreateMap<Property, PropertySummaryDto>();
        CreateMap<Property, PropertyDetailDto>();
        CreateMap<Tenant, TenantDto>();
        CreateMap<Document, DocumentDto>();
        CreateMap<PropertyCreateDto, Property>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<PropertyUpdateDto, Property>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore());
    }
}
