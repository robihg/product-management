using framework.BaseService.Helpers;
using framework.BaseService.Interfaces.Jwt;
using framework.DTO.BaseDTO.GenericResponse;
using framework.DTO.GeneralSettingDTO.Requests;
using framework.DTO.GeneralSettingDTO.Responses;
using framework.GeneralSetting.Interfaces.ModificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.DataAccess.Models.Base;
using ProductManagement.DataAccess.Models.GeneralSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.GeneralSetting.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RefUserController : ControllerBase
    {
        #region CONSTRUCTOR
        private readonly IRefUserService _refUserService;
        private readonly IJwtService _jwtService;

        public RefUserController(IRefUserService refUserService, IJwtService jwtService)
        {
            _refUserService = refUserService;
            _jwtService = jwtService;
        }
        #endregion

        #region Transaction
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResGeneric<ResRefUser>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResGeneric<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResGeneric<object>))]
        public virtual async Task<IActionResult> AddRefUser([FromBody] ReqRefUser reqRefUser)
        {
            if (!ModelState.IsValid)
                return ResponseHelper.BuildValidationErrorResponse(ModelState);

            try
            {
                var refUser = await _refUserService.AddEditRefUser(reqRefUser);
                return ResponseHelper.BuildSuccessResponse(refUser);
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
