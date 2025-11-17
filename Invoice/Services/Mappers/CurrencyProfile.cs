using invoice.Core.DTO.Currency;
using invoice.Core.Entities;
using AutoMapper;

namespace invoice.Services.Mappers
{
    public class CurrencyProfile : Profile
    {
        public CurrencyProfile()
        {
            CreateMap<Currency, CurrencyReadDTO>().ReverseMap();
        }
    }
}

