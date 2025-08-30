using AutoMapper;
using invoice.Core.DTO.Tax;
using invoice.Core.Entites;

namespace invoice.Services.Mappers
{
    public class TaxProfile:Profile
    {
        public TaxProfile()
        {
            CreateMap<Tax, TaxReadDTO>().ReverseMap(); 
        }

    }
}
