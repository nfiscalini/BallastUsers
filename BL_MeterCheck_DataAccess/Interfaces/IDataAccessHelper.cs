using System.Data;

namespace BL_MeterCheck_DataAccess.Interfaces
{
    public interface IDataAccessHelper
    {
        Task<DataSet> ExecuteDataset(string command, CommandType commandType, DataAccessParameters parameters, string connectionString = "");

        Task<int> ExecuteNonQuery(string storeProcedure, DataAccessParameters parameters, string connectionString);

        Task<int> ExecuteNonQueryReturnValue(string storeProcedure, DataAccessParameters parameters, string connectionString, string parametroSalida);

        Task<int> ExecuteNonQueryReturnValue(string storeProcedure, DataAccessParameters parameters, string connectionString);

        Task<DataSet> ExecuteText(string strQuery, DataAccessParameters parameters, string connectionString);
    }
}
