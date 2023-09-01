using Api.Users.Services;
using BL_MeterCheck_DataAccess;
using BL_MeterCheck_DataAccess.Extensions;
using BL_MeterCheck_DataAccess.Interfaces;
using BL_MeterCheckModels;
using BL_MeterCheckModels.DTO;
using System.Data;

namespace Api.Users.Business
{
    public class UserAdministrator : IUserAdministrator
    {
        private readonly ILogger<UserAdministrator> _logger;
        private readonly IDataAccessHelper _dataAccessHelper;

        public UserAdministrator(IDataAccessHelper dataAccessHelper, ILogger<UserAdministrator> logger) 
        {
            _dataAccessHelper = dataAccessHelper;
            _logger = logger;
        }

        public async Task<int> AddUserAsync(UserModel model)
        {
            string hashedPassword = PasswordService.GetHashedPassword(model.Password);
            var parameters = new DataAccessParameters();
            int user_id = 0;

            try
            {
                parameters.AddStringParameter("NAMES", 200, model.Names);
                parameters.AddStringParameter("LASTNAME", 200, model.Lastname);
                parameters.AddStringParameter("LOGINNAME", 200, model.Loginname);
                parameters.AddStringParameter("PASSWORD", 1000, hashedPassword);

                DataSet data = await _dataAccessHelper.ExecuteDataset("[dbo].[PR_ADD_USER]", CommandType.StoredProcedure, parameters);

                if (data != null && data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0) 
                {
                    DataTable dataTable = data.Tables[0];
                    user_id = Convert.ToInt32(dataTable.Rows[0][0]);
                }

                return user_id;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at UserIsValidAsync", ex);
                throw;
            }
        }

        public async Task<ResponseModel<UserModel>> GetUserAsync(string loginName, string password)
        {
            string hashedPassword = PasswordService.GetHashedPassword(password);
            var parameters = new DataAccessParameters();
            ResponseModel<UserModel> responseModel = null;

            try
            {
                parameters.AddStringParameter("LOGINNAME", 200, loginName);
                parameters.AddStringParameter("PASSWORD", 1000, hashedPassword);

                DataSet data = await _dataAccessHelper.ExecuteDataset("[dbo].[PR_GET_USER_BY_CREDENTIALS]", CommandType.StoredProcedure, parameters);

                if (data != null && data.Tables.Count > 0)
                {
                    DataTable dataTable = data.Tables[0];
                    responseModel = new ResponseModel<UserModel>(dataTable.Rows[0].To<UserModel>());
                }

                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetUserAsync", ex);
                throw;
            }
        }
    }
}
