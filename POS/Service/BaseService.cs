using POS.DAL;
using POS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace POS.Service
{
    public class BaseService 
    {
        protected BaseDal baseDal = new BaseDal();

        #region DB operations
        /// <summary>
        /// Returns a friendly string. If obj is DBNull or null, it returns an empty string.
        /// </summary>
        /// <param name="obj">String, DBNull or null.</param>
        /// <returns></returns>
        public static string GetDBString(object obj)
        {
            if (!(obj is DBNull) && obj != null)
            {
                return Convert.ToString(obj);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns a friendly integer. If obj is DBNull or null, it returns 0.
        /// </summary>
        /// <param name="obj">Can be a string, DBNull or null.</param>
        /// <returns>0 if param was DBNull or null.</returns>
        public static int GetDBInt(object obj)
        {
            if (!(obj is DBNull) && obj != null)
            {
                return Convert.ToInt32(obj);
            }
            else
            {
                return 0;
            }
        }

        #endregion

        protected ErrorMessage AddSQLError(int ErrNumber, string ErrMsg)
        {
            ErrorMessage em = new ErrorMessage();

            try
            {
                if (baseDal.ErrCode == 0)
                {
                    em.ErrNumber = 0;
                    em.Message = "SUCCESS";
                }
                else
                {
                    em.ErrNumber = ErrNumber;
                    em.Message = ErrMsg;
                }
                em.AppError = false;
                return em;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected ErrorMessage AddApplicationError(Exception ex)
        {
            ErrorMessage em = new ErrorMessage();

            em.AppError = true;
            em.ErrNumber = ex.HResult;
            em.Message = ex.Message;
            return em;
        }

        #region Open DB Connection
        /// <summary>
        /// Open DataBase Connection for given dbName
        /// </summary>
        /// <param name="dbName"></param>
        public void OpenConnection(string dbName)
        {
            baseDal.ConnectName = dbName;
            baseDal.SQLConnect();
            baseDal.ClearParameters();
        }
        #endregion
    }
}