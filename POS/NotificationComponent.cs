using Microsoft.AspNet.SignalR;
using POS.DAL;
using POS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace POS
{
    public class NotificationComponent
    {
        #region Register Notification
        /// <summary>
        /// RegisterNotification 
        /// </summary>
        /// <param name="currentTime">Gets the current time</param>
        public void RegisterNotification(DateTime currentTime)
        {
            string conStr = ConfigurationManager.ConnectionStrings["notificationString"].ConnectionString;
            string sqlCommand = @"select [Id],[OrderName],[Description] from [dbo].[PurchaseOrder] where [TimeStamp] > @TimeStamp";

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);
                cmd.Parameters.AddWithValue("@TimeStamp", currentTime);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDependency = new SqlDependency(cmd);
                sqlDependency.OnChange += sqlDependency_OnChange;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                }
            }
        }
        #endregion

        #region On DB Change
        /// <summary>
        /// Event is fired if there is any change in db associated with the Sql command that is set
        /// </summary>
        /// <param name="sender">Sender as params</param>
        /// <param name="e">SqlNotificationEventArgs</param>
        void sqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDependency = sender as SqlDependency;
                sqlDependency.OnChange -= sqlDependency_OnChange;

                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("newnotification");

                RegisterNotification(DateTime.Now);
            }
        }
        #endregion

        #region Get Recent Db Chages
        /// <summary>
        /// Gets the recent changes made in DB 
        /// </summary>
        /// <param name="afterDate">Changes made in DB after the mentioned date</param>
        /// <returns></returns>
        public List<POSModel> GetRecentChanges(DateTime afterDate)
        {
            List<POSModel> list = new List<POSModel>();
            POSDal posDal = new POSDal();
            list = posDal.Notification(afterDate);
            return list;
        }
        #endregion
    }
}