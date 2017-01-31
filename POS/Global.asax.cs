using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace POS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //Connection string for the SignalR notification
        string con = ConfigurationManager.ConnectionStrings["notificationString"].ConnectionString;

        /// <summary>
        /// Application Start
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SqlDependency.Start(con);
        }

        /// <summary>
        /// Session Start
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        protected void Session_Start(object sender, EventArgs e)
        {
            NotificationComponent nc = new NotificationComponent();
            var currentTime = DateTime.Now;
            HttpContext.Current.Session["LastUpdated"] = currentTime;
            nc.RegisterNotification(currentTime);

        }

        /// <summary>
        /// Application End
        /// </summary>
        protected void Application_End()
        {
            SqlDependency.Stop(con);
        }
    }
}
