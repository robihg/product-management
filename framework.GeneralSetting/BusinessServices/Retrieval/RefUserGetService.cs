using AutoMapper;
using framework.BaseService.Interfaces.Jwt;
using framework.BaseService.Repository;
using framework.GeneralSetting.Interfaces.Retrieval;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductManagement.DataAccess.DataContexts.GeneralSettings;
using ProductManagement.DataAccess.Models.GeneralSettings;


namespace framework.GeneralSetting.BusinessServices.Retrieval
{
   public class RefUserGetService : IRefUserGetService
    {
        #region CONSTRUCTOR
        private readonly GeneralSettingContext _context;
        private readonly IRepository<GeneralSettingContext> _repository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RefUserGetService(
            GeneralSettingContext context,
            IRepository<GeneralSettingContext> repository,
            IMapper mapper,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region GET 
        public async Task<RefUser?> GetRefUserByGuid(Guid guid)
        {
            var result = (from a in _context.RefUsers
                          where a.Guid == guid
                          select a).FirstOrDefaultAsync();

            return await result;
        }
        #endregion
    }
}
