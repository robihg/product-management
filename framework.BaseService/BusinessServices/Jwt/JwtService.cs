using AutoMapper;
using framework.BaseService.Interfaces.Jwt;
using framework.BaseService.Models.Jwt;
using framework.BaseService.Repository;
using framework.DTO.GeneralSettingDTO.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.DataAccess.DataContexts.GeneralSettings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace framework.BaseService.BusinessServices.Jwt
{
    public class JwtService : IJwtService
    {
        #region CONSTRUCTOR
        private readonly GeneralSettingContext _context;
        private readonly IRepository<GeneralSettingContext> _repositorySettings;
        private readonly IMapper _mapper;
        private readonly JwtModel _jwtModels;
        #endregion

        public JwtService(IOptions<JwtModel> jwtModels, GeneralSettingContext context, IRepository<GeneralSettingContext> repositorySetting, IHttpContextAccessor ContextAccessor, IMapper mapper)
        {
            _jwtModels = jwtModels.Value;
            _context = context;
            _repositorySettings = repositorySetting;
            _mapper = mapper;
        }

        public async Task<ResRefUser> Authenticated(string username, string password)
        {
            string hashedPassword = ComputeSha256Hash(password);
            var user = await _context.RefUsers
                .Where(x => x.Username == username && x.Password == hashedPassword)
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Username and password not match");
            return _mapper.Map<ResRefUser>(user);
        }

        public string ComputeSha256Hash(string rawData)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new();

            foreach (byte t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }

            return builder.ToString();
        }


        public string GenerateToken(long userId, string username, string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtModels.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _jwtModels.Issuer,
                audience: _jwtModels.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtModels.ExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
