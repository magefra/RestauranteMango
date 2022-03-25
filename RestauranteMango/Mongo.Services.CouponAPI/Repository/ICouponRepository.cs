using Mongo.Services.CouponAPI.Models.Dtos;

namespace Mongo.Services.CouponAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
