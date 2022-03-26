
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

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

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCouponAsync<ResponseDto>(cartDto, accessToken);



            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }


            return View();
        }


        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCouponAsync<ResponseDto>(cartDto.CartHeader.UserId, accessToken);



            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }


            return View();
        }

        private async Task<CartDto> LoadCartDtoBaseOnLoggedInUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
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

        public async Task<IActionResult> Remove(int CartDetailId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCartAsync<ResponseDto>(CartDetailId, accessToken);



            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }
    }
}
