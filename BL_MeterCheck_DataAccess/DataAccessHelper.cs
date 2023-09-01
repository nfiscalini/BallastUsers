using BL_MeterCheck_DataAccess.Interfaces;
using BL_MeterCheckModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BL_MeterCheck_DataAccess
{
    public class DataAccessHelper : IDataAccessHelper
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IDataAccessHelper> _logger;

        public DataAccessHelper(IConfiguration configuration, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor, ILogger<IDataAccessHelper> logger)
        {
            _configuration = configuration;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<DataSet> ExecuteDataset(string command, CommandType commandType, DataAccessParameters parameters, string connectionString = "")
        {
            StringBuilder datos = new();

            if (string.IsNullOrEmpty(connectionString)) 
            {
                connectionString = "DefaultConnection";
            }

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString(connectionString)))
                {
                    await connection.OpenAsync();
                    using (var cmd = new SqlCommand(command, connection))
                    {
                        cmd.CommandTimeout = 120;

                        if (parameters != null)
                        {
                            foreach (SqlParameter param in parameters.ParamerersList)
                            {
                                datos.Append("Parametro : " + param.ParameterName + " Valor: " + param.SqlValue);
                                cmd.Parameters.Add(param);
                            }
                        }
                        cmd.CommandType = commandType;
                        var adapter = new SqlDataAdapter(cmd);

                        DataSet dataSet = new DataSet();
                        await Task.Run(() =>
                        {
                            adapter.Fill(dataSet);
                        });

                        return dataSet;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteDataset: " + command + " " + datos.ToString(), ex);
                throw new Exception("DataAccessHelper.ExecuteDataset Error", ex);
            }
        }

        public async Task<int> ExecuteNonQuery(string storeProcedure, DataAccessParameters parameters, string connectionString)
        {
            int result = -1;

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString(connectionString)))
                {
                    await connection.OpenAsync();
                    using (var cmd = new SqlCommand(storeProcedure, connection))
                    {
                        cmd.CommandTimeout = 120;

                        if (parameters != null)
                        {
                            foreach (SqlParameter param in parameters.ParamerersList)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }
                        cmd.CommandType = CommandType.StoredProcedure;
                        result = await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteNonQuery: " + storeProcedure, ex);
                throw new Exception("DataAccessHelper.ExecuteNonQuery: " + storeProcedure, ex);
            }

            return result;
        }

        public async Task<int> ExecuteNonQueryReturnValue(string storeProcedure, DataAccessParameters parameters, string connectionString, string parametroSalida)
        {
            int result = -1;

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString(connectionString)))
                {
                    await connection.OpenAsync();
                    using (var cmd = new SqlCommand(storeProcedure, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SqlParameter param in parameters.ParamerersList)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }
                        cmd.Parameters.Add(parametroSalida, SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        await cmd.ExecuteNonQueryAsync();
                        result = (int)cmd.Parameters[cmd.Parameters.Count - 1].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteNonQueryReturnValue: " + storeProcedure, ex);
                throw new Exception("DataAccessHelper.ExecuteNonQueryReturnValue Error", ex);
            }

            return result;
        }

        public async Task<int> ExecuteNonQueryReturnValue(string storeProcedure, DataAccessParameters parameters, string connectionString)
        {
            int result = -1;

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString(connectionString)))
                {
                    await connection.OpenAsync();
                    using (var cmd = new SqlCommand(storeProcedure, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SqlParameter param in parameters.ParamerersList)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }
                        var retValue = cmd.Parameters.Add("return", SqlDbType.Int);
                        retValue.Direction = ParameterDirection.ReturnValue;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        await cmd.ExecuteNonQueryAsync();
                        result = Convert.ToInt32(retValue.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteNonQueryReturnValue: " + storeProcedure, ex);
                throw new Exception("DataAccessHelper.ExecuteNonQueryReturnValue Error", ex);
            }

            return result;
        }

        public async Task<DataSet> ExecuteText(string strQuery, DataAccessParameters parameters, string connectionString)
        {
            StringBuilder datos = new StringBuilder();

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString(connectionString)))
                {
                    await connection.OpenAsync();
                    using (var cmd = new SqlCommand(strQuery, connection))
                    {
                        cmd.CommandTimeout = 120;

                        if (parameters != null)
                        {
                            foreach (SqlParameter param in parameters.ParamerersList)
                            {
                                datos.Append("Parametro : " + param.ParameterName + " Valor: " + param.SqlValue);
                                cmd.Parameters.Add(param);
                            }
                        }
                        cmd.CommandType = System.Data.CommandType.Text;
                        var adapter = new SqlDataAdapter(cmd);

                        DataSet dataSet = new DataSet();
                        await Task.Run(() =>
                        {
                            adapter.Fill(dataSet);
                        });
                        return dataSet;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ExecuteText: " + strQuery + " " + datos.ToString(), ex);
                throw new Exception("DataAccessHelper.ExecuteText Error", ex);
            }
        }
    }
}
