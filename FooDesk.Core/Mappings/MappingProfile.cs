using AutoMapper;
using FooDesk.Core.Dto;
using FooDesk.Core.Entities;

namespace FooDesk.Core.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();
        }
    }
}
