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
        public IActionResult BuildSuccessResponse(List<T> data)
        {
            var response = new ResGeneric<T>
            {
                HeaderObj = new HeaderObj
                {
                    ResponseTime = DateTime.Now.ToString("o"),
                    StatusCode = "200",
                    Message = "Success"
                },
                Data = data ?? new List<T>()
            };

            return new OkObjectResult(response);
        }

        public IActionResult BuildNotFoundResponse(string message = "Data Not Found")
        {
            var errorResponse = new ResGeneric<object>
            {
                HeaderObj = new HeaderObj
                {
                    ResponseTime = DateTime.Now.ToString(),
                    StatusCode = "404",
                    Message = message
                },
                Data = []
            };

            return new NotFoundObjectResult(errorResponse);
        }

        public IActionResult BuildErrorResponse(Exception ex)
        {
            var errorResponse = new ResGeneric<object>
            {
                HeaderObj = new HeaderObj
                {
                    ResponseTime = DateTime.Now.ToString("o"),
                    StatusCode = "500",
                    Message = "Error",
                    ErrorMessages = new List<ValidationError>
                    {
                       new ValidationError(ex.Message, "Internal Error")
                    }
                },
                Data = []
            };

            return new ObjectResult(errorResponse) { StatusCode = 500 };
        }
    }
}
