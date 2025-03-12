
using framework.DTO.BaseDTO.GenericResponse;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.DataAccess.Models.Base;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;


namespace framework.BaseService.Helpers
{
    public static class ResponseHelper
    {
        public static IActionResult? ValidateSearchFilters(decimal? minPrice, decimal? maxPrice)
        {
            if (minPrice < 0) return BuildCustomErrorResponse("Min price must be at least 0.", StatusCodes.Status400BadRequest);
            if (maxPrice < 0) return BuildCustomErrorResponse("Max price must be at least 0.", StatusCodes.Status400BadRequest);
            if (minPrice > maxPrice) return BuildCustomErrorResponse("Min price cannot be greater than max price.", StatusCodes.Status400BadRequest);

            return null;
        }

        public static IActionResult BuildCustomErrorResponse(string message, int statusCode)
        {
            var errorResponse = new ResGeneric<object>
            {
                HeaderObj = new HeaderObj
                {
                    StatusCode = statusCode.ToString(),
                    Message = message,
                    ResponseTime = DateTime.Now.ToString("o"),
                    ErrorMessages = new List<ValidationError> { new ValidationError(message) }
                },
                Data = null
            };

            return new ObjectResult(errorResponse) { StatusCode = statusCode };
        }

        public static IActionResult BuildSuccessResponse<T>(T data) where T : class?
        {
            return new OkObjectResult(new ResGeneric<T>
            {
                HeaderObj = new HeaderObj
                {
                    StatusCode = "200",
                    Message = "Success",
                    ResponseTime = DateTime.Now.ToString("o"),
                    ErrorMessages = null
                },
                Data = data
            });
        }

        public static IActionResult BuildValidationErrorResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Keys
                       .SelectMany(key => modelState[key].Errors
                           .Select(error => new ValidationError(error.ErrorMessage, key)))
                       .ToList();

            return new BadRequestObjectResult(new ResGeneric<object>
            {
                HeaderObj = new HeaderObj
                {
                    StatusCode = "400",
                    Message = "Validation failed",
                    ResponseTime = DateTime.Now.ToString("o"),
                    ErrorMessages = errors
                },
                Data = null
            });
        }

        public static IActionResult BuildNotFoundResponse(string message = "Data Not Found")
        {
            return new NotFoundObjectResult(new ResGeneric<object>
            {
                HeaderObj = new HeaderObj
                {
                    StatusCode = "404",
                    Message = message,
                    ResponseTime = DateTime.Now.ToString("o"),
                    ErrorMessages = null
                },
                Data = null
            });
        }

        public static IActionResult BuildErrorResponse(Exception ex)
        {
            return new ObjectResult(new ResGeneric<object>
            {
                HeaderObj = new HeaderObj
                {
                    StatusCode = "500",
                    Message = "An unexpected error occurred.",
                    ResponseTime = DateTime.Now.ToString("o"),
                    ErrorMessages = new List<ValidationError> { new ValidationError(ex.Message) }
                },
                Data = null
            })
            { StatusCode = 500 };
        }
    }

}
