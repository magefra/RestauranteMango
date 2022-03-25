using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartAPI.Models
{

    public class CartDetails
    {

        [Key]
        public int CartDetailId { get; set; }

        public int CartHeaderId { get; set; }

        //[ForeignKey("CartHeaderId")]

        public CartHeader CartHeader { get; set; }

        public int ProductId { get; set; }

        //[ForeignKey("ProductId")]

        public Product Product { get; set; }

        public int Count { get; set; }
    }
}
