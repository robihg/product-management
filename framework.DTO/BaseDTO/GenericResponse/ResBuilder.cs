using Microsoft.AspNetCore.Mvc;
using ProductManagement.DataAccess.Models.Base;

namespace framework.DTO.BaseDTO.GenericResponse
{
    public class ResBuilder<T>
    {
        public IActionResult BuildSuccessResponse(T data)
        {
            return BuildSuccessResponse(new List<T> { data });
        }
        public static IActionResult BuildSuccessResponse<T>(T data)
        {
            var response = new ResGeneric<T>
            {
                HeaderObj = new HeaderObj
                {
                    StatusCode = "200",
                    Message = "Success",
                    ResponseTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ErrorMessages = null
                },
                Data = data ?? (typeof(T) == typeof(List<T>) ? (T)(object)new List<T>() : default)
            };

            return new OkObjectResult(response);
        }

        public IActionResult BuildNotFoundResponse(string message = "Data Not Found")
        {
            var errorResponse = new ResGeneric<object>
            {
                HeaderObj = new HeaderObj
                {
                    ResponseTime = DateTime.UtcNow.ToString("o"),
                    StatusCode = "404",
                    Message = message
                },
                Data = null
            };

            return new NotFoundObjectResult(errorResponse);
        }

        public IActionResult BuildErrorResponse(Exception ex)
        {
            var errorResponse = new ResGeneric<object>
            {
                HeaderObj = new HeaderObj
                {
                    ResponseTime = DateTime.UtcNow.ToString("o"),
                    StatusCode = "500",
                    Message = "Error",
                    ErrorMessages = new List<ValidationError>
            {
                new ValidationError(ex.Message, "Internal Error")
            }
                },
                Data = null 
            };

            return new ObjectResult(errorResponse) { StatusCode = 500 };
        }

    }
}
