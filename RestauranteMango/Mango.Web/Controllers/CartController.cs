
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
        private readonly ICouponervice _couponervice;

        public CartController(IProductService productService,
                              ICartService cartService,
                              ICouponervice couponervice)
        {
            _productService = productService;
            _cartService = cartService;
            _couponervice = couponervice;
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

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBaseOnLoggedInUser());
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

                if (!string.IsNullOrEmpty(card.CartHeader.CouponCode))
                {
                    var coupon = await _couponervice.GetCouponAsync<ResponseDto>(
                        card.CartHeader.CouponCode, accessToken);
                    if (coupon != null && coupon.IsSuccess)
                    {
                        var couponObj = JsonConvert.DeserializeObject<CouponDto>(
                            Convert.ToString(coupon.Result));

                        card.CartHeader.DiscountTotal = couponObj.DiscountAmount;

                    }
                }

                foreach (var detail in card.CartDetails)
                {
                    card.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
                }


                card.CartHeader.OrderTotal -= card.CartHeader.DiscountTotal;
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
