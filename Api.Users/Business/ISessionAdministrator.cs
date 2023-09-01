using BL_MeterCheckModels;

namespace Api.Users.Business
{
    public interface ISessionAdministrator
    {
        Task<bool> AddSessionAsync(UserModel model, string token);
    }
}
