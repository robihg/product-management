using AutoMapper;
using framework.BaseService.Interfaces.Jwt;
using framework.BaseService.Repository;
using framework.DTO.BaseDTO.GenericRequest;
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
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            ProductContext context,
            IRepository<ProductContext> repository,
            IMapper mapper,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor,
            IProductGetService productGetService,
            ILogger<ProductService> logger)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _productGetService = productGetService;
            _logger = logger;


        }
        #endregion

        #region ADD EDIT DELETE PRODUCT
        public virtual async Task<ResProduct> AddEditProduct(ReqProduct reqProduct)
        {
            try
            {
                _logger.LogInformation("Adding/Editing product: {ProductName}", reqProduct.Name);

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
                _logger.LogInformation("Success Adding/Editing product: {ProductName}", reqProduct.Name);

                return _mapper.Map<ResProduct>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding product {ProductName}", reqProduct.Name);
                return null; 
            }
           
        }
        public virtual async Task<ResProduct> DeleteProduct(ReqByGuidObj guid)
        {
            ProductManagement.DataAccess.Models.Products.Product product = await _productGetService.GetProductByGuid(guid);

            if (product == null)
            {
                _logger.LogWarning("Product not found for deletion: {0}", guid.Guid);
                return null;
            }
            _repository.Remove(product);
            await _repository.SaveChangesAsync();
            _logger.LogWarning("Deleted product: {0}", guid.Guid);
            return _mapper.Map<ResProduct>(product);
        }
        #endregion
    }
}
