using AutoMapper;
using InvoiceSystem.Application.DTOs;
using InvoiceSystem.Domain.Entities;

namespace InvoiceSystem.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<Customer, CustomerDto>();
        CreateMap<InvoiceItem, InvoiceItemResponseDto>();
        CreateMap<Invoice, InvoiceResponseDto>();
        
        // DTO to Entity mappings
        CreateMap<CreateInvoiceItemDto, InvoiceItem>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
        
        CreateMap<CreateInvoiceDto, Invoice>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}
