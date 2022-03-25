
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {

        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public CartController(IProductService productService,
                              ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public object CardDto { get; private set; }

        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBaseOnLoggedInUser());
        }

        private async Task<CartDto> LoadCartDtoBaseOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("acces_token");
            var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

            CartDto card = new CartDto();

            if (response != null && response.IsSuccess)
            {
                card = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }

            if (card.CartHeader != null)
            {
                foreach (var detail in card.CartDetails)
                {
                    card.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
                }
            }

            return card;

        }
    }
}
