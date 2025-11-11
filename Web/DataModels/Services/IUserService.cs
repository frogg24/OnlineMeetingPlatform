using DataModels.Models;
using DataModels.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Services
{
    public interface IUserService
    {
        Task<List<UserViewModel>?> ReadList(UserSearchModel? model);
        Task<UserViewModel?> ReadElement(UserSearchModel model);
        Task<bool> Create(UserViewModel model);
        Task<bool> Update(UserViewModel model);
        Task<bool> Delete(UserViewModel model);

    }
}
