using framework.BaseService.Interfaces.Jwt;
using framework.DTO.BaseDTO.GenericResponse;
using framework.DTO.GeneralSettingDTO.Requests;
using framework.DTO.GeneralSettingDTO.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace framework.BaseService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region CONSTRUCTOR
        private readonly IJwtService _jwtService;

        public AuthController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }
        #endregion

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResGeneric<ResRefUser>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResGeneric<object>))]
        public async Task<IActionResult> Login([FromBody] ReqLogin reqLogin)
        {
            try
            {
                string username = reqLogin.Username;
                string password = reqLogin.Password;

                var user = await _jwtService.Authenticated(username, password);
                var token = _jwtService.GenerateToken(user.Id, user.Username, user.Email);

                return Ok(new { Token = token });
            }
            catch (KeyNotFoundException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                var responseService = new ResBuilder<object>();
                return responseService.BuildErrorResponse(ex);
            }
        }
    }
}
