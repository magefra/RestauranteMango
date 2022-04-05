using Mango.Services.OrderApi.DbContexts;
using Mango.Services.OrderApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderApi.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;

        public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            _db.Entry(orderHeader).State = EntityState.Added;
            _db.OrderHeaders.Add(orderHeader);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }


            foreach (var orderDetails in orderHeader.OrderDetails)
            {
                orderDetails.OrderHeaderId = orderHeader.OrderHeaderId;

                _db.Entry(orderDetails).State = EntityState.Added;
                _db.OrderDetails.Add(orderDetails);
                await _db.SaveChangesAsync();
            }

            return true;
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            var orderHeaderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == orderHeaderId);
            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.PaymentStatus = paid;
                _db.Entry(orderHeaderFromDb).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
        }
    }
}
