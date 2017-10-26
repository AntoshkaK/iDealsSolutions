using iDealsSolutions.DbData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iDealsSolutions.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string error)
        { 
            return View();                     
        }       
        public ActionResult Error(string error)
        {
            ViewBag.Error = error;
            return View();
        }
    }
}