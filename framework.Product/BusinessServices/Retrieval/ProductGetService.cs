using AutoMapper;
using framework.BaseService.Interfaces.Jwt;
using framework.BaseService.Repository;
using framework.DTO.BaseDTO.GenericRequest;
using framework.DTO.ProductDTO.Responses;
using framework.Product.Interfaces.Retrieval;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductManagement.DataAccess.DataContexts.Products;
using ProductManagement.DataAccess.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.Product.BusinessServices.Retrieval
{
    public class ProductGetService : IProductGetService
    {
        #region CONSTRUCTOR
        private readonly ProductContext _context;
        private readonly IRepository<ProductContext> _repository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductGetService(
            ProductContext context,
            IRepository<ProductContext> repository,
            IMapper mapper,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region GET 
        public async Task<ProductManagement.DataAccess.Models.Products.Product?> GetProductByGuid(ReqByGuidObj reqGuid)
        {
            var result = (from a in _context.Products
                          where a.Guid == reqGuid.Guid
                          select a).FirstOrDefaultAsync();

            return await result;
        }

        public async Task<List<ResProduct>> SearchProducts(string? name, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            List<ProductManagement.DataAccess.Models.Products.Product?> productResult = await query.ToListAsync();
            return _mapper.Map<List<ResProduct>>(productResult);
        }
        #endregion

    }
}
