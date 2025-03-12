using framework.DTO.GeneralSettingDTO.Requests;
using ProductManagement.DataAccess.Models.GeneralSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.GeneralSetting.Interfaces.ModificationService
{
   public interface IRefUserService
    {
        Task<RefUser> AddEditRefUser(ReqRefUser reqRefUser);
    }
}
