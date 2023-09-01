using Api.Users.Services;
using BL_MeterCheck_DataAccess;
using BL_MeterCheck_DataAccess.Interfaces;
using BL_MeterCheckModels;
using System.Data;

namespace Api.Users.Business
{
    public class SessionAdministrator : ISessionAdministrator
    {
        private readonly ILogger<SessionAdministrator> _logger;
        private readonly IDataAccessHelper _dataAccessHelper;

        public SessionAdministrator(IDataAccessHelper dataAccessHelper, ILogger<SessionAdministrator> logger)
        {
            _dataAccessHelper = dataAccessHelper;
            _logger = logger;
        }

        public async Task<bool> AddSessionAsync(UserModel model, string token)
        {
            var parameters = new DataAccessParameters();
            bool result = false;

            try
            {
                parameters.AddIntParameter("USER_ID", model.User_id);
                parameters.AddStringParameter("TOKEN", 1000, token);

                DataSet data = await _dataAccessHelper.ExecuteDataset("[dbo].[PR_ADD_SESSION]", CommandType.StoredProcedure, parameters);

                if (data != null && data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
                {
                    DataTable dataTable = data.Tables[0];
                    result = Convert.ToBoolean(dataTable.Rows[0][0]);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at AddSessionAsync", ex);
                throw;
            }
        }

    }
}
