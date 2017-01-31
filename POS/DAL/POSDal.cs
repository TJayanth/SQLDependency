using POS.Models;
using POS.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace POS.DAL
{
    public class POSDal : BaseService
    {
        #region CRUD
        /// <summary>
        /// Creates a new Purchase Order
        /// </summary>
        /// <param name="posModel">POSModel object as params</param>
        /// <returns>Returns id of the PO</returns>
        public int Create(POSModel posModel)
        {
            ErrorMessage em = new ErrorMessage();
            try
            {
                baseDal.ConnectName = "POSConnection";
                baseDal.SQLConnect();
                baseDal.ClearParameters();

                baseDal.AddParameter("@in_OrderName", posModel.OrderName, false);
                baseDal.AddParameter("@in_Description", posModel.Description, false);

                return Convert.ToInt32(baseDal.ExecuteScalar("sp_AddPurchaseOrder"));
            }
            catch (Exception ex)
            {
                em = AddApplicationError(ex);
                return -1;
            }
            finally
            {
                baseDal.SQLDisconnect();
            }
        }

        /// <summary>
        /// Gets all PO details 
        /// </summary>
        /// <returns>Returns List of PO</returns>
        public List<POSModel> Read()
        {
            List<POSModel> posList = new List<POSModel>();
            try
            {
                OpenConnection("POSConnection");
                return baseDal.GetList<POSModel>("sp_ReadPurchaseOrder");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                baseDal.SQLDisconnect();
            }
        }

        /// <summary>
        /// Updates the PO for the given id
        /// </summary>
        /// <param name="posModel">PO model object</param>
        /// <returns>Returns int if successfully updated</returns>
        public int Update(POSModel posModel)
        {
            ErrorMessage em = new ErrorMessage();
            try
            {
                baseDal.ConnectName = "POSConnection";
                baseDal.SQLConnect();
                baseDal.ClearParameters();

                baseDal.AddParameter("@in_OrderName", posModel.OrderName, false);
                baseDal.AddParameter("@in_Description", posModel.Description, false);
                baseDal.AddParameter("@in_Id", posModel.Id, false);

                return Convert.ToInt32(baseDal.ExecuteScalar("sp_UpdatePurchaseOrder"));
            }
            catch (Exception ex)
            {
                em = AddApplicationError(ex);
                return -1;
            }
            finally
            {
                baseDal.SQLDisconnect();
            }
        }

        /// <summary>
        /// Deletes the employee for the given id
        /// </summary>
        /// <param name="posModel">POSModel object</param>
        /// <returns>Returns 1 if deleted, else returns -1</returns>
        public int Delete(POSModel posModel)
        {
            try
            {
                OpenConnection("POSConnection");
                baseDal.AddParameter("@in_Id", posModel.Id, false);

                baseDal.Get<POSModel>("sp_DeletePurchaseOrder");
                return 1;
            }
            catch
            {
                return -1;
            }
            finally
            {
                baseDal.SQLDisconnect();
            }
        }
        #endregion

        #region Get PO details
        /// <summary>
        /// Gets PO details for the given id
        /// </summary>
        /// <param name="posModel">POS Model</param>
        /// <returns>Returns PO details</returns>
        public POSModel GetPurchaseOrder(POSModel posModel)
        {
            try
            {
                OpenConnection("POSConnection");
                baseDal.AddParameter("@in_Id", posModel.Id , false);

                return baseDal.Get<POSModel>("sp_GetPurchaseOrder");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                baseDal.SQLDisconnect();
            }
        }
        #endregion

        #region Recent 5 changes 
        /// <summary>
        /// Gets recent PO details
        /// </summary>
        /// <returns>Returns List of posModel</returns>
        public List<POSModel> Recent()
        {
            List<POSModel> empList = new List<POSModel>();
            try
            {
                OpenConnection("POSConnection");
                return baseDal.GetList<POSModel>("sp_RecentPurchaseOrder");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                baseDal.SQLDisconnect();
            }
        }
        #endregion

        #region Notification
        /// <summary>
        /// Gets Notification value for any change in db
        /// </summary>
        /// <param name="time">Time as params</param>
        /// <returns>Returns list of POSModel</returns>
        public List<POSModel> Notification(DateTime time)
        {
            List<POSModel> empList = new List<POSModel>();
            try
            {
                OpenConnection("POSConnection");
                baseDal.AddParameter("@in_Timestamp", time, false);
                return baseDal.GetList<POSModel>("sp_Notification");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                baseDal.SQLDisconnect();
            }
        }
        #endregion
    }
}

