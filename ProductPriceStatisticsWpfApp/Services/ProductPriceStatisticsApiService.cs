using Newtonsoft.Json;
using ProductPriceStatistics.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductPriceStatisticsWpfApp.Services
{
    class ProductPriceStatisticsApiService : IProductPriceStatisticsApiService, IDisposable
    {
        private readonly string _appUri;
        private readonly HttpClient _httpClient;


        public ProductPriceStatisticsApiService(string appUri)
        {
            _appUri = appUri;
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var response = await _httpClient.GetAsync($"{_appUri}/api/product");
            var jsonBody = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(jsonBody);

            return products;
        }

        public async Task<IEnumerable<PriceDto>> GetPricesOfProduct(Guid productId, DateTime? startDateTime, DateTime? finishDateTime)
        {
            var response = await _httpClient.GetAsync($"{_appUri}/api/Price?productId={productId}");
            var jsonBody = await response.Content.ReadAsStringAsync();
            var prices = JsonConvert.DeserializeObject<IEnumerable<PriceDto>>(jsonBody);

            return prices;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
