using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class CartHeader
    {

        public int CartHeaderId { get; set; }
        public string UserId { get; set; }

        public string CouponCode { get; set; }

        [NotMapped]
        public List<CartDetails> CartDetails { get; set; }
    }
}
