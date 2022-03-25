using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mongo.Services.CouponAPI.DbContexts;
using Mongo.Services.CouponAPI.Models.Dtos;

namespace Mongo.Services.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {

        private readonly ApplicationDbContext _db;
        protected IMapper _mapper;
        public CouponRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var couponFromDb = await _db.Coupons.FirstOrDefaultAsync(u => u.CouponCode == couponCode);
            return _mapper.Map<CouponDto>(couponFromDb);
        }
    }
}
