using framework.BaseService.Helpers;
using framework.BaseService.Interfaces.Jwt;
using framework.DTO.BaseDTO.GenericRequest;
using framework.DTO.BaseDTO.GenericResponse;
using framework.DTO.GeneralSettingDTO.Requests;
using framework.DTO.GeneralSettingDTO.Responses;
using framework.DTO.ProductDTO.Requests;
using framework.DTO.ProductDTO.Responses;
using framework.Product.Interfaces.Modificaiton;
using framework.Product.Interfaces.Retrieval;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.Product.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        #region CONSTRUCTOR
        private readonly IProductService _productService;
        private readonly IProductGetService _productGetService;
        private readonly IJwtService _jwtService;

        public ProductController(
            IProductService productService,
            IProductGetService productGetService,
            IJwtService jwtService)
        {
            _productService = productService;
            _productGetService = productGetService;
            _jwtService = jwtService;
        }
        #endregion

        #region Get Data
        [Authorize]
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResGeneric<List<ResProduct>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResGeneric<object>))]
        public async Task<IActionResult> SearchProducts([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var validationError = ResponseHelper.ValidateSearchFilters(minPrice, maxPrice);
            if (validationError != null) return validationError;

            try
            {
                var products = await _productGetService.SearchProducts(name, minPrice, maxPrice);
                return products.Any() ? ResponseHelper.BuildSuccessResponse(products) : ResponseHelper.BuildNotFoundResponse("No products found.");
            }
            catch (Exception ex)
            {
                return ResponseHelper.BuildErrorResponse(ex);
            }
        }
        #endregion
        #region Transaction
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResGeneric<ResProduct>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResGeneric<object>))]
        public virtual async Task<IActionResult> Product([FromBody] ReqProduct reqProduct)
        {
            if (!ModelState.IsValid)
                return ResponseHelper.BuildValidationErrorResponse(ModelState);

            try
            {
                var product = await _productService.AddEditProduct(reqProduct);
                return ResponseHelper.BuildSuccessResponse(product);
            }
            catch (KeyNotFoundException ex)
            {
                return ResponseHelper.BuildNotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ResponseHelper.BuildErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResGeneric<ResProduct>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResGeneric<object>))]
        public virtual async Task<IActionResult> Delete([FromBody] ReqByGuidObj reqByGuidObj)
        {
            if (!ModelState.IsValid)
                return ResponseHelper.BuildValidationErrorResponse(ModelState);

            try
            {
                var product = await _productService.DeleteProduct(reqByGuidObj);
                if(product == null)
                     return ResponseHelper.BuildValidationErrorResponse(ModelState);

                return ResponseHelper.BuildSuccessResponse(product);
            }
            catch (KeyNotFoundException ex)
            {
                return ResponseHelper.BuildNotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ResponseHelper.BuildErrorResponse(ex);
            }
        }
        #endregion
    }
}
