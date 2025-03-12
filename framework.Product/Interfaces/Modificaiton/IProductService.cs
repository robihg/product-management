using framework.DTO.BaseDTO.GenericRequest;
using framework.DTO.ProductDTO.Requests;
using framework.DTO.ProductDTO.Responses;
using System.Threading.Tasks;


namespace framework.Product.Interfaces.Modificaiton
{
    public interface IProductService
    {
        Task<ResProduct> AddEditProduct(ReqProduct reqProduct);
        Task<ResProduct> DeleteProduct(ReqByGuidObj guid);
    }
}
