namespace Mango.Web.Services.IServices
{
    public interface ICouponervice
    {
        Task<T> GetCouponAsync<T>(string couponCode, string token = null);
    }
}
