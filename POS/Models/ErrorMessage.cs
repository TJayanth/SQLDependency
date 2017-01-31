using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS.Models
{
    public class ErrorMessage
    {
        public Boolean AppError { get; set; }
        public long ErrNumber { get; set; }
        public String Message { get; set; }
    }
}