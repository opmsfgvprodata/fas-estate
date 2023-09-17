using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Models
{
    public class MVC_SYSTEM_Models_Config : DbConfiguration
    {
        public MVC_SYSTEM_Models_Config()
        {
            AddInterceptor(new StringTrimmerInterceptor());
        }
    }
}