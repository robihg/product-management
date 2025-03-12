using framework.BaseService.Interfaces.Jwt;
using framework.DTO.BaseDTO.GenericResponse;
using framework.DTO.GeneralSettingDTO.Requests;
using framework.DTO.GeneralSettingDTO.Responses;
using framework.DTO.ProductDTO.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace framework.BaseService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region CONSTRUCTOR
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IJwtService jwtService,
             ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _logger = logger;
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
                _logger.LogInformation("trying to login with username: {0}", reqLogin.Username);
                string username = reqLogin.Username;
                string password = reqLogin.Password;

                var user = await _jwtService.Authenticated(username, password);
                var token = _jwtService.GenerateToken(user.Id, user.Username, user.Email);

                _logger.LogInformation("Successfuly login with username: {0}", reqLogin.Username);
                return Ok(new { Token = token });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Unauthorized login by : {0}", reqLogin.Username);
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Login errpor by username : {0}", reqLogin.Username);
                var responseService = new ResBuilder<object>();
                return responseService.BuildErrorResponse(ex);
            }
        }
    }
}
