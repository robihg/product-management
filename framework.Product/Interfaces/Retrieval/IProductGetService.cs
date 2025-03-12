using framework.BaseService.Models;
using framework.DTO.BaseDTO.GenericRequest;
using framework.DTO.ProductDTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.Product.Interfaces.Retrieval
{
    public interface IProductGetService
    {
       Task<ProductManagement.DataAccess.Models.Products.Product?> GetProductByGuid(ReqByGuidObj guid);
       Task<List<ResProduct>> SearchProducts(string? name, decimal? minPrice, decimal? maxPrice);
       Task<PagedResult<ResProduct>> GetPaginatedProducts(PaginationParams paginationParams);
    }
}
