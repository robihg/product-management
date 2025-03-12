using ProductManagement.DataAccess.Models.GeneralSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.GeneralSetting.Interfaces.Retrieval
{
    public interface IRefUserGetService
    {
        Task<RefUser?> GetRefUserByGuidForAddEdit(Guid guid);
    }
}
