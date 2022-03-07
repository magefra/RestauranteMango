using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory _clientFactory;

        public ProductService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _clientFactory = httpClient;
        }

        public async Task<T> CreateProductAsync<T>(ProductDto productDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                apiType = SD.ApiType.POST,
                Data = productDto,
                Url = SD.ProductAPIBase + "/api/products",
                AccessToken = ""
            });
        }

        public async Task<T> DeleteProductAsync<T>(int id)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                apiType = SD.ApiType.DELETE,
                Url = SD.ProductAPIBase + $"/api/products/{id}",
                AccessToken = ""
            });
        }

        public async Task<T> GetAllProductsAsync<T>()
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + $"/api/products",
                AccessToken = ""
            });
        }

        public async Task<T> GetProductByIdAsync<T>(int id)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                apiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + $"/api/products/{id}",
                AccessToken = ""
            });
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto productDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                apiType = SD.ApiType.PUT,
                Data = productDto,
                Url = SD.ProductAPIBase + $"/api/products",
                AccessToken = ""
            });
        }
    }
}
