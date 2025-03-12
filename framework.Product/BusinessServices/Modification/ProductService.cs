using AutoMapper;
using framework.BaseService.Interfaces.Jwt;
using framework.BaseService.Repository;
using framework.DTO.BaseDTO.GenericRequest;
using framework.DTO.GeneralSettingDTO.Responses;
using framework.DTO.ProductDTO.Requests;
using framework.DTO.ProductDTO.Responses;
using framework.Product.Interfaces.Modificaiton;
using framework.Product.Interfaces.Retrieval;
using Microsoft.AspNetCore.Http;
using ProductManagement.DataAccess.DataContexts.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.Product.BusinessServices.Modification
{
    public class ProductService : IProductService
    {
        #region CONSTRUCTOR
        private readonly ProductContext _context;
        private readonly IRepository<ProductContext> _repository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductGetService _productGetService;

        public ProductService(
            ProductContext context,
            IRepository<ProductContext> repository,
            IMapper mapper,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor,
            IProductGetService productGetService)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _productGetService = productGetService;


        }
        #endregion

        #region ADD EDIT DELETE PRODUCT
        public virtual async Task<ResProduct> AddEditProduct(ReqProduct reqProduct)
        {
            ProductManagement.DataAccess.Models.Products.Product? product = null;
            if (reqProduct.Guid.HasValue)
            {
                product = await _productGetService.GetProductByGuid(new ReqByGuidObj { Guid = reqProduct.Guid.Value });
            }

            if (product == null)
            {
                product = _mapper.Map<ProductManagement.DataAccess.Models.Products.Product>(reqProduct);
                product.Guid = Guid.NewGuid();

                _repository.Add(product);
            }
            else
            {
                _mapper.Map(reqProduct, product);
            }

            await _repository.SaveChangesAsync();

            return _mapper.Map<ResProduct>(product);
        }
        public virtual async Task<ResProduct> DeleteProduct(ReqByGuidObj guid)
        {
            ProductManagement.DataAccess.Models.Products.Product product = await _productGetService.GetProductByGuid(guid);

            if (product == null)
            {
                return null;
            }
            _repository.Remove(product);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ResProduct>(product);
        }
        #endregion
    }
}
