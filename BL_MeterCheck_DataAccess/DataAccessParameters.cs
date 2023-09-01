using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL_MeterCheck_DataAccess
{
    public class DataAccessParameters
    {
        #region Properties

        public List<SqlParameter> ParamerersList { get; set; }

        #endregion

        #region Constructors

        public DataAccessParameters()
        {
            ParamerersList = new List<SqlParameter>();
        }

        #endregion

        #region Public Methods

        public void AddGuidParameter(string name, Guid? value)
        {
            SqlParameter paramGuid = new SqlParameter(name, SqlDbType.UniqueIdentifier)
            {
                IsNullable = true
            };
            if (value.HasValue)
            {
                paramGuid.Value = value;
            }
            else
            {
                paramGuid.Value = DBNull.Value;
            }

            ParamerersList.Add(paramGuid);
        }

        public void AddIntParameter(string name, int value)
        {
            SqlParameter paramInteger = new SqlParameter(name, SqlDbType.Int)
            {
                Value = value
            };

            ParamerersList.Add(paramInteger);
        }

        public void AddXmlParameter(string name, string value)
        {
            SqlParameter paramXML = new SqlParameter(name, SqlDbType.Xml)
            {
                Value = value
            };

            ParamerersList.Add(paramXML);
        }

        public void AddBitParameter(string name, bool value)
        {
            SqlParameter paramBit = new SqlParameter(name, SqlDbType.Bit)
            {
                Value = (value ? 1 : 0)
            };

            ParamerersList.Add(paramBit);
        }

        public void AddIntNullParameter(string name, int? value)
        {
            SqlParameter paramInteger = new SqlParameter(name, SqlDbType.Int)
            {
                IsNullable = true
            };
            if (value.HasValue)
            {
                paramInteger.Value = value;
            }
            else
            {
                paramInteger.Value = null;
            }

            ParamerersList.Add(paramInteger);
        }

        public void AddDecimalParameter(string name, decimal value, byte precision, byte scale)
        {
            SqlParameter paramInteger = new SqlParameter(name, SqlDbType.Decimal)
            {
                Value = value
            };
            if (scale != 0)
            {
                paramInteger.Scale = scale;
            }
            if (precision != 0)
            {
                paramInteger.Precision = precision;
            }

            ParamerersList.Add(paramInteger);
        }

        public void AddFloatParameter(string name, float value)
        {
            SqlParameter paramInteger = new SqlParameter(name, SqlDbType.Float)
            {
                Value = value
            };
 
            ParamerersList.Add(paramInteger);
        }

        public void AddStringParameter(string name, int size, string? value)
        {
            #region Avoid SQL injection attack: p.e. ' or 1=1 --
            string comillas = "\"\"";

            if (value != null && value.IndexOf("'") >= 0)
            {
                value = value.Replace("'", string.Empty).Trim() == string.Empty ? comillas : value;
                value = value.Replace("'", string.Empty);
            }

            if (value != null && value.IndexOf("%") >= 0)
            {
                value = value.Replace("%", string.Empty).Trim() == string.Empty ? comillas : value;
                value = value.Replace("%", string.Empty);
            }

            value = value != null && value.Trim() == "" && value.Trim() != value ? null : value;
            #endregion

            SqlParameter paramString = new SqlParameter(name, SqlDbType.NVarChar, size)
            {
                Value = value
            };

            ParamerersList.Add(paramString);
        }

        public void AddTypeParameter(string name, string typeName, DataTable dt)
        {
            SqlParameter param = new SqlParameter(name, SqlDbType.Structured)
            {
                TypeName = typeName,
                Value = dt
            };

            ParamerersList.Add(param);
        }

        public void AddBinaryParameter(string name, byte[] value)
        {
            SqlParameter paramString = new SqlParameter(name, SqlDbType.VarBinary, value.Length)
            {
                Value = value
            };

            ParamerersList.Add(paramString);
        }

        public void AddImageParameter(string name, byte[] value)
        {
            SqlParameter paramString = new SqlParameter(name, SqlDbType.Image, value.Length)
            {
                Value = value
            };

            ParamerersList.Add(paramString);
        }

        public void AddNullableStringParameter(string name, int size, string? value)
        {
            SqlParameter paramString;

            #region Avoid SQL injection attack: p.e. ' or 1=1 --
            string comillas = "\"\"";

            if (value != null && value.IndexOf("'") >= 0)
            {
                value = value.Replace("'", string.Empty).Trim() == string.Empty ? comillas : value;
                value = value.Replace("'", string.Empty);
            }

            if (value != null && value.IndexOf("%") >= 0)
            {
                value = value.Replace("%", string.Empty).Trim() == string.Empty ? comillas : value;
                value = value.Replace("%", string.Empty);
            }

            value = value != null && value.Trim() == "" && value.Trim() != value ? null : value;
            #endregion

            if (value == null)
            {
                paramString = new SqlParameter(name, DBNull.Value)
                {
                    Value = DBNull.Value
                };
            }
            else
            {
                paramString = new SqlParameter(name, SqlDbType.NVarChar, size)
                {
                    Value = value
                };
            }

            ParamerersList.Add(paramString);
        }

        public void AddNullableDatetimeParameter(string name, DateTime? value)
        {
            SqlParameter paramDatetime;

            if (value == null)
            {
                paramDatetime = new SqlParameter(name, DBNull.Value)
                {
                    Value = DBNull.Value
                };
            }
            else
            {
                paramDatetime = new SqlParameter(name, SqlDbType.DateTime)
                {
                    Value = value
                };
            }

            ParamerersList.Add(paramDatetime);
        }

        public void AddDatetimeParameter(string name, DateTime value)
        {
            SqlParameter paramDatetime = new SqlParameter(name, SqlDbType.DateTime)
            {
                Value = value
            };
            ParamerersList.Add(paramDatetime);
        }

        public void AddDatetimeParameterFromStr(string name, string value)
        {
            try
            {
                DateTime dtValue = DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SqlParameter paramDatetime;
                paramDatetime = new SqlParameter(name, SqlDbType.DateTime)
                {
                    Value = dtValue
                };
                ParamerersList.Add(paramDatetime);
            }
            catch (Exception)
            {
                SqlParameter paramDatetime = new SqlParameter(name, SqlDbType.DateTime)
                {
                    IsNullable = true,
                    Value = DBNull.Value
                };
                ParamerersList.Add(paramDatetime);
            }
        }

        #endregion
    }
}
