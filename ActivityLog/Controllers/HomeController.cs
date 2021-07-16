using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ActivityLog.Models;
using PagedList;

namespace ActivityLog.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize]
        [ActionName("Index")]
        public ActionResult ActivityList(string sortOrder, string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var activity = from s in db.activityModels
                           select s;
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                activity = activity.Where(s =>
                s.Log.ToUpper().Contains(searchString.ToUpper())
                ||
                s.Id.ToString().Contains(searchString));
            }
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(activity.OrderByDescending(s => s.dateTime).ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
        public ActionResult Delete(int id)
        {
            ActivityModel activity = db.activityModels.Find(id);
            db.activityModels.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}