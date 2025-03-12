using AutoMapper;
using framework.BaseService.Interfaces.Jwt;
using framework.BaseService.Repository;
using framework.DTO.BaseDTO.GenericRequest;
using framework.DTO.GeneralSettingDTO.Requests;
using framework.DTO.GeneralSettingDTO.Responses;
using framework.GeneralSetting.Interfaces.ModificationService;
using framework.GeneralSetting.Interfaces.Retrieval;
using Microsoft.AspNetCore.Http;
using ProductManagement.DataAccess.DataContexts.GeneralSettings;
using ProductManagement.DataAccess.Models.GeneralSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.GeneralSetting.BusinessServices.Modification
{
    public class RefUserService : IRefUserService
    {
        #region CONSTRUCTOR
        private readonly GeneralSettingContext _context;
        private readonly IRepository<GeneralSettingContext> _repository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRefUserGetService _refUserGetService;
        
        public RefUserService(
            GeneralSettingContext context,
            IRepository<GeneralSettingContext> repository,
            IMapper mapper,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor,
            IRefUserGetService refUserGetService)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _refUserGetService = refUserGetService;
        }
        #endregion

        #region ADD EDIT DELETE REF USER
        public virtual async Task<RefUser> AddEditRefUser(ReqRefUser reqRefUser)
        {
            RefUser ru = reqRefUser.Guid.HasValue ? await _refUserGetService.GetRefUserByGuidForAddEdit(reqRefUser.Guid.Value) : null;

            if (ru == null)
            {
                var newUser = new RefUser
                {
                    Guid = Guid.NewGuid(),
                    Name = reqRefUser.Name,
                    Username = reqRefUser.Username,
                    Email = reqRefUser.Email,
                    IsActive = reqRefUser.IsActive,
                    Password = _jwtService.ComputeSha256Hash(reqRefUser.Password)
                };

                _repository.Add(newUser);
                ru = newUser;
            }
            else
            {
                ru.Name = reqRefUser.Name;
                ru.Username = reqRefUser.Username;
                ru.Email = reqRefUser.Email;
                ru.Password = _jwtService.ComputeSha256Hash(reqRefUser.Password);
            }

            await _repository.SaveChangesAsync();
            return ru;
        }
        #endregion
    }
}
