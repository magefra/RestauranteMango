using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class Couponervice : BaseService, ICouponervice
    {

        private readonly IHttpClientFactory _httpClient;

        public Couponervice(IHttpClientFactory httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<T> GetCouponAsync<T>(string couponCode, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.CoupinAPIBase + $"/api/coupon/{couponCode}",
                AccessToken = token
            });
        }
    }
}
