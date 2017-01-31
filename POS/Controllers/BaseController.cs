using POS.DAL;
using POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POS.Controllers
{
    public class BaseController : Controller
    {
        #region Instance Creation
        /// <summary>
        /// Instance for POSDal
        /// </summary>
        private POSDal _posDal;
        public POSDal POSDal
        {
            get
            {
                if (_posDal == null)
                {
                    _posDal = new POSDal();
                    return _posDal;
                }
                else
                {
                    return _posDal;
                }
            }
        }

        /// <summary>
        /// Instance for POSModel
        /// </summary>
        private POSModel _posModel;
        public POSModel POSModel
        {
            get
            {
                if (_posModel == null)
                {
                    _posModel = new POSModel();
                    return _posModel;
                }
                else
                {
                    return _posModel;
                }
            }
            set
            {
                _posModel = value;
            }
        }
        #endregion
    }
}