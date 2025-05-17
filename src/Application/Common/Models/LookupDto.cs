using InvoiceManagementAPI.Domain.Entities;

namespace InvoiceManagementAPI.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }
    public string? Title { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Customer, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));

            CreateMap<Product, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));
        }
    }
}
