using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ActivityLog.Models;
namespace ActivityLog.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [ActionName("Index")]
        public ActionResult ActivityList()
        {
            return View(db.activityModels.ToList());
        }
    }
}