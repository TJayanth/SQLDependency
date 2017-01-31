using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS.Models
{
    public class POSModel
    {
        /// <summary>
        /// Id of PO
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// PO order name
        /// </summary>
        public string OrderName { get; set; }

        /// <summary>
        /// Created Time Stamp of the order
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// Description of the PO
        /// </summary>
        public string Description { get; set; }
    }
}