using framework.DTO.GeneralSettingDTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.BaseService.Interfaces.Jwt
{
    public interface IJwtService
    {
        Task<ResRefUser> Authenticated(string username, string password);
        string ComputeSha256Hash(string rawData);
        string GenerateToken(long userId, string username, string email);
    }
}
