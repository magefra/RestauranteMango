namespace Mango.Services.OrderApi.Repository
{
    public interface IEmailRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);
        Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);
    }
}
