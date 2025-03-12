using AutoMapper;
using framework.DTO.GeneralSettingDTO.Responses;
using ProductManagement.DataAccess.Models.GeneralSettings;

namespace framework.DTO.Mapping.GeneralSettings
{
    public class MappingProfileGeneralSetting : Profile
    {
        public MappingProfileGeneralSetting()
        {
            CreateMap<RefUser, ResRefUser>();
        }
    }
}
