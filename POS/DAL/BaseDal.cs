using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace POS.DAL
{
    public class OutputParameter
    {
        public String ParameterName;
        public Object ParameterValue;
    }

    public class BaseDal
    {
        public String ConnectName;
        public Int32 ErrCode;
        public String ErrMsg;
        public List<OutputParameter> OutputParameters = null;
        private SqlConnection SQLCn = new SqlConnection();
        private SqlCommand cmd;
        private SqlDataAdapter sda;
        private SqlDataReader sdr;
        private SqlParameter prm;
        private SqlTransaction Trans;
        private List<SqlParameter> Parameters = new List<SqlParameter>();
        private DataTable dt = new DataTable();

        /// <summary>
        /// Opens connection 
        /// </summary>
        public void SQLConnect()
        {
            try
            {
                if (SQLCn.State == System.Data.ConnectionState.Open)
                {
                    SQLCn.Close();
                }

                SQLCn = new SqlConnection();
                SQLCn.ConnectionString = ConfigurationManager.ConnectionStrings[ConnectName].ToString();
                SQLCn.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Disconnects SQL
        /// </summary>
        public void SQLDisconnect()
        {
            try
            {
                if (SQLCn.State == System.Data.ConnectionState.Open)
                {
                    SQLCn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add Parameter for the sql query
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <param name="ParameterValue"></param>
        /// <param name="OutputParameter"></param>
        public void AddParameter(String ParameterName, Object ParameterValue, Boolean OutputParameter)
        {
            try
            {
                prm = new SqlParameter(ParameterName, ParameterValue);
                if (OutputParameter)
                {
                    prm.Direction = ParameterDirection.Output;
                    prm.Size = 256;
                }
                Parameters.Add(prm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Clear Parameters
        /// </summary>
        public void ClearParameters()
        {
            Parameters.Clear();
        }

        public DataTable GetSQLDatatable(String StoredProc)
        {

            try
            {
                dt = new DataTable();
                cmd = new SqlCommand(StoredProc, SQLCn);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter prm in Parameters)
                {
                    cmd.Parameters.Add(prm);
                }
                sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Object ExecuteScalar(String StoredProc)
        {
            Object obj = new Object();
            try
            {
                cmd = new SqlCommand(StoredProc, SQLCn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = Trans;
                foreach (SqlParameter prm in Parameters)
                {
                    cmd.Parameters.Add(prm);
                }
                obj = cmd.ExecuteScalar();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ExecuteSQLNonQuery(String StoredProc)
        {
            OutputParameter op;
            OutputParameters = new List<OutputParameter>();
            try
            {
                ErrCode = 0;
                ErrMsg = "";
                cmd = new SqlCommand(StoredProc, SQLCn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;
                cmd.Transaction = Trans;

                foreach (SqlParameter prm in Parameters)
                {
                    cmd.Parameters.Add(prm);
                }

                cmd.ExecuteNonQuery();

                OutputParameters.Clear();
                foreach (SqlParameter prm in cmd.Parameters)
                {
                    if (prm.Direction == ParameterDirection.Output)
                    {
                        if (prm.ParameterName == "@ErrCode")
                        {
                            ErrCode = Convert.ToInt32(prm.Value);
                        }
                        if (prm.ParameterName == "@ErrMsg")
                        {
                            ErrMsg = Convert.ToString(prm.Value);
                        }
                        op = new OutputParameter();
                        op.ParameterName = prm.ParameterName;
                        op.ParameterValue = prm.Value;
                        OutputParameters.Add(op);

                    }
                }
            }
            catch (Exception ex)
            {
                foreach (SqlParameter prm in cmd.Parameters)
                {
                    if (prm.Direction == ParameterDirection.Output)
                    {
                        if (prm.ParameterName == "@ErrCode")
                        {
                            ErrCode = Convert.ToInt32(prm.Value);
                        }
                        if (prm.ParameterName == "@ErrMsg")
                        {
                            ErrMsg = Convert.ToString(prm.Value);
                        }
                        op = new OutputParameter();
                        op.ParameterName = prm.ParameterName;
                        op.ParameterValue = prm.Value;
                        OutputParameters.Add(op);

                    }
                }
                if (ErrCode == 0)
                {
                    ErrCode = 1;
                    throw ex;
                }
            }
            finally
            {

            }
        }

        public List<T> GetList<T>(String StoredProc) where T : new()
        {
            IDataReader dr = null;
            try
            {
                cmd = new SqlCommand(StoredProc, SQLCn);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter prm in Parameters)
                {
                    cmd.Parameters.Add(prm);
                }

                dr = cmd.ExecuteReader();

                return ToList<T>(dr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }

        public List<T> ToList<T>(IDataReader datareader) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                for (int i = 0; i < datareader.FieldCount; i++)
                {
                    columnsNames.Add(datareader.GetName(i));
                }

                while (datareader.Read())
                {
                    T myT = getObject<T>((IDataRecord)datareader, columnsNames);
                    Temp.Add(myT);
                }

                return Temp;
            }
            catch { return Temp; }
        }

        public T getObject<T>(IDataRecord row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties; Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = row[columnname].ToString().Replace("%", "");
                            objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                        }
                    }
                }

                return obj;
            }
            catch { return obj; }
        }

        public T Get<T>(String StoredProc) where T : new()
        {
            IDataReader dr = null;
            T Temp = new T();
            try
            {
                cmd = new SqlCommand(StoredProc, SQLCn);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter prm in Parameters)
                {
                    cmd.Parameters.Add(prm);
                }

                dr = cmd.ExecuteReader();

                List<string> columnsNames = new List<string>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    columnsNames.Add(dr.GetName(i));
                }

                if (dr.Read())
                {
                    Temp = getObject<T>((IDataRecord)dr, columnsNames);
                }

                return Temp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }
    }
}