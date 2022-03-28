using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public interface ICartService
    {
        Task<T> GetCartByUserIdAsync<T>(string userId, string token = null);
        Task<T> AddCartAsync<T>(CartDto cartDto, string token = null);
        Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null);
        Task<T> RemoveCartAsync<T>(int cartId, string token = null);

        Task<T> ApplyCouponAsync<T>(CartDto cartDto, string token = null);
        Task<T> RemoveCouponAsync<T>(string userId, string token = null);

        Task<T> Checout<T>(CartHeaderDto cartHeader, string token = null);
    }
}
