using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models
{

    public class CartDetails
    {

        [Key]
        public int CartDetailId { get; set; }

        public int CartHeaderId { get; set; }

        //[ForeignKey("CartHeaderId")]
        [NotMapped]
        public CartHeader CartHeader { get; set; }

        public int ProductId { get; set; }

        //[ForeignKey("ProductId")]
        [NotMapped]
        public Product Product { get; set; }

        public int Count { get; set; }
    }
}
