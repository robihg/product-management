using AutoMapper;
using framework.DTO.ProductDTO.Requests;
using framework.DTO.ProductDTO.Responses;
using ProductManagement.DataAccess.Models.Products;

namespace framework.DTO.Mapping.Products
{
    public class MappingProfileProduct : Profile
    {
        public MappingProfileProduct()
        {
            CreateMap<ReqProduct, Product>();
            CreateMap<Product, ResProduct>();
        }
    }
}
