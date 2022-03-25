using Microsoft.EntityFrameworkCore;
using Mongo.Services.CouponAPI.Models;

namespace Mongo.Services.CouponAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Coupon> Coupons { get; set; }
    }
}
