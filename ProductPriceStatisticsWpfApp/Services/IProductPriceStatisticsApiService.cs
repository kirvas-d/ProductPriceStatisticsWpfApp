using ProductPriceStatistics.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPriceStatisticsWpfApp.Services
{
    interface IProductPriceStatisticsApiService
    {
        Task<IEnumerable<ProductDto>> GetProducts();

        Task<IEnumerable<PriceDto>> GetPricesOfProduct(Guid productId, DateTime? startDateTime, DateTime? finishDateTime);
    }
}
