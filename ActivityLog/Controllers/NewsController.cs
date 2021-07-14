using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ActivityLog.Models;

namespace ActivityLog.Controllers
{
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: News
        public ActionResult Index()
        {
            if (Session["Id"] != null)
            {
                var newsModels = db.newsModels.Include(n => n.category);
                return View(newsModels.ToList());
            }
            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel user)
        {
            user.Password = GetSHA256(user.Password);
            var checkUser = db.userModels.SingleOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            if (checkUser != null)
            {
                Session["Id"] = checkUser.Id;
                Session["Username"] = checkUser.Username;
                ActivityModel write = new ActivityModel();
                string writeactivity = "Đã đăng nhập vào hệ thống";
                write.UserId = checkUser.Id;
                write.dateTime = DateTime.Now;
                write.Log = writeactivity;
                db.activityModels.Add(write);
                db.SaveChanges();
                return RedirectToAction("ManagePassword");
            }
            else
            {
                ModelState.AddModelError("", "Username or password does not exists");
            }
            return View();
        }
        public ActionResult LogOff()
        {
            ActivityModel write = new ActivityModel();
            string writeactivity = "Đã đăng xuất khỏi hệ thống";
            int userid = (int)Session["Id"];
            write.UserId = userid;
            write.dateTime = DateTime.Now;
            write.Log = writeactivity;
            db.activityModels.Add(write);
            db.SaveChanges();
            Session.Clear();
            return RedirectToAction("Login");
        }
        public ActionResult ManagePassword()
        {
            return View();
        }
        public ActionResult ChangedPassword()
        {
            return RedirectToAction("ChangedPassword");
        }
        static string GetSHA256(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                NewsModel newsModel = db.newsModels.Find(id);
                if (newsModel == null)
                {
                    return HttpNotFound();
                }
                return View(newsModel);
            }
            return RedirectToAction("Login");
        }

        // GET: News/Create
        public ActionResult Create()
        {
            if (Session["Id"] != null)
            {
                ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name");
                return View();
            }
            return RedirectToAction("Login");
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Noidung,CategoryId")] NewsModel newsModel)
        {
            if (ModelState.IsValid)
            {
                db.newsModels.Add(newsModel);
                db.SaveChanges();
                ActivityModel write = new ActivityModel();
                string writeactivity = "Đã tạo mới một bài viết, id bài viết: "+newsModel.Id;
                int userid = (int)Session["Id"];
                write.UserId = userid;
                write.dateTime = DateTime.Now;
                write.Log = writeactivity;
                db.activityModels.Add(write);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", newsModel.CategoryId);
            return View(newsModel);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                NewsModel newsModel = db.newsModels.Find(id);
                if (newsModel == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", newsModel.CategoryId);
                return View(newsModel);
            }
            return RedirectToAction("Login");
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Noidung,CategoryId")] NewsModel newsModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newsModel).State = EntityState.Modified;
                ActivityModel write = new ActivityModel();
                string writeactivity = "Đã chỉnh sửa một bài viết, id bài viết: " + newsModel.Id;
                int userid = (int)Session["Id"];
                write.UserId = userid;
                write.dateTime = DateTime.Now;
                write.Log = writeactivity;
                db.activityModels.Add(write);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", newsModel.CategoryId);
            return View(newsModel);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                NewsModel newsModel = db.newsModels.Find(id);
                if (newsModel == null)
                {
                    return HttpNotFound();
                }
                return View(newsModel);
            }
            return RedirectToAction("Login");
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NewsModel newsModel = db.newsModels.Find(id);
            db.newsModels.Remove(newsModel);
            ActivityModel write = new ActivityModel();
            string writeactivity = "Đã xoá một bài viết, id bài viết: " + newsModel.Id;
            int userid = (int)Session["Id"];
            write.UserId = userid;
            write.dateTime = DateTime.Now;
            write.Log = writeactivity;
            db.activityModels.Add(write);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
