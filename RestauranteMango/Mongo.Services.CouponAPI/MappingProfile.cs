using AutoMapper;
using Mongo.Services.CouponAPI.Models;
using Mongo.Services.CouponAPI.Models.Dtos;

namespace Mongo.Services.CouponAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CouponDto, Coupon>().ReverseMap();
        }
    }
}
