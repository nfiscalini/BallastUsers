using BL_MeterCheckModels;
using BL_MeterCheckModels.DTO;

namespace Api.Users.Business
{
    public interface IUserAdministrator
    {
        Task<int> AddUserAsync(UserModel model);
        Task<ResponseModel<UserModel>> GetUserAsync(string loginName, string password);
    }
}
