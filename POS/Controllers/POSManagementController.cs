using POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace POS.Controllers
{
    public class POSManagementController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region CRUD
        /// <summary>
        /// Create View for purchase order
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates new purchase order for the given data
        /// </summary>
        /// <param name="posModel">Gets posModel</param>
        /// <returns>Returns read</returns>
        [HttpPost]
        public ActionResult Create(POSModel posModel)
        {
            POSDal.Create(posModel);
            return RedirectToAction("Read");
        }

        /// <summary>
        /// Edits the Purchase order details for the given id
        /// </summary>
        /// <param name="id">Purchase order id</param>
        /// <returns>Returns the read</returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            POSModel.Id = id;
            POSModel = POSDal.GetPurchaseOrder(POSModel);
            return View("Create", POSModel);
        }

        /// <summary>
        /// Displays details for the given Purchase order id
        /// </summary>
        /// <param name="id">Purchase order id</param>
        /// <returns>Returns Purchase order details for the given id</returns>
        [HttpGet]
        public ActionResult Details(int id)
        {
            POSModel.Id = id;
            POSModel = POSDal.GetPurchaseOrder(POSModel);
            return View(POSModel);
        }

        /// <summary>
        /// Edit Purchase order details for the given id
        /// </summary>
        /// <param name="posModel">POSModel object</param>
        /// <returns>After successful updation redirects to Read method</returns>
        public ActionResult Edit(POSModel posModel)
        {
            POSDal.Update(posModel);
            return RedirectToAction("Read");
        }

        /// <summary>
        /// Read all Purchase order details
        /// </summary>
        /// <returns>Returns List of Purchase order</returns>
        public ActionResult Read()
        {
            List<POSModel> empList = new List<POSModel>();
            empList = POSDal.Read();
            return View(empList);
        }

        /// <summary>
        /// Deletes Purchase order details for the given id
        /// </summary>
        /// <param name="id">Purchase order id</param>
        /// <returns>Returns View with Purchase order details for delete</returns>
        [HttpGet]
        public ActionResult Delete(int id)
        {
            POSModel.Id = id;
            POSModel = POSDal.GetPurchaseOrder(POSModel);
            return View(POSModel);
        }

        /// <summary>
        /// Deletes the Purchase order for the given model
        /// </summary>
        /// <param name="posModel">Purchase order object</param>
        /// <returns>After deleting the records redirects to Read method </returns>
        [HttpPost]
        public ActionResult Delete(POSModel posModel)
        {
            POSDal.Delete(posModel);
            return RedirectToAction("Read");
        }
        #endregion

        #region Read Recent 5
        /// <summary>
        /// Get Recent changes through SqlCacheDependency
        /// </summary>
        /// <returns>Returns Partial view</returns>
        public ActionResult Recent()
        {
            List<POSModel> empList = new List<POSModel>();
            if (System.Web.HttpContext.Current.Cache["RecentPos"] != null)
            {
                empList = System.Web.HttpContext.Current.Cache["RecentPos"] as List<POSModel>;
            }
            else
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;
                System.Web.Caching.SqlCacheDependencyAdmin.EnableNotifications(connStr);
                System.Web.Caching.SqlCacheDependencyAdmin.EnableTableForNotifications(connStr, "PurchaseOrder");

                SqlCacheDependency sqlDependency = new SqlCacheDependency("CachingPOS", "PurchaseOrder");
                empList = POSDal.Recent();
                System.Web.HttpContext.Current.Cache.Insert("RecentPos", empList, sqlDependency);
            }

            return PartialView(empList);
        }

        #endregion

        #region Get Notification
        /// <summary>
        /// Get Notification
        /// </summary>
        /// <returns>Returns JSON Result of recent changes</returns>
        public JsonResult GetNotification()
        {
            var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
            NotificationComponent notificationComponent = new NotificationComponent();
            var list = notificationComponent.GetRecentChanges(notificationRegisterTime);

            return new JsonResult { Data = list, JsonRequestBehavior=JsonRequestBehavior.AllowGet };
        }
        #endregion
    }
}