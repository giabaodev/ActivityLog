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
            if (Session["Id"] != null && db.userModels.Find((int)Session["Id"]) != null)
            {
                var newsModels = db.newsModels.Include(n => n.category);
                return View(newsModels.ToList());
            }
            Session["Id"] = null;
            Session["Username"] = null;
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
                Session["Username"] = checkUser.Hoten;
                int id = (int)Session["Id"];
                var users = db.userModels.Find(id);
                if (Session["Auditing"] != null)
                {
                    if (users.Theodoi == true)
                    {
                        ActivityModel write = new ActivityModel();
                        string writeactivity = "Đã đăng nhập vào hệ thống";
                        write.UserId = checkUser.Id;
                        write.dateTime = DateTime.Now;
                        write.Log = writeactivity;
                        db.activityModels.Add(write);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("ManageAccount");
            }
            else
            {
                ModelState.AddModelError("", "Sai thông tin đăng nhập hoặc người dùng đã bị xoá.");
            }
            return View();
        }
        public ActionResult LogOff()
        {
            int id = (int)Session["Id"];
            var user = db.userModels.Find(id);
            if (user != null)
            {
                if (Session["Auditing"] != null)
                {
                    if (user.Theodoi == true)
                    {
                        ActivityModel write = new ActivityModel();
                        string writeactivity = "Đã đăng xuất khỏi hệ thống";
                        write.UserId = id;
                        write.dateTime = DateTime.Now;
                        write.Log = writeactivity;
                        db.activityModels.Add(write);
                        db.SaveChanges();
                    }
                }
                Session["Id"] = null;
                return RedirectToAction("Login");
            }
            else
            {
                Session["Id"] = null;
                Session["Username"] = null;
                return RedirectToAction("Login");
            }
        }
        public ActionResult ManageAccount()
        {
            if (Session["Id"] != null && db.userModels.Find((int)Session["Id"]) != null)
            {
                UserModel userModel = db.userModels.Find(Session["Id"]);
                return View(userModel);
            }
            Session["Id"] = null;
            Session["Username"] = null;
            return RedirectToAction("Login");
        }
        public ActionResult ManagePassword(int? id)
        {
            if (Session["Id"] != null && db.userModels.Find((int)Session["Id"]) != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserModel userModel = db.userModels.Find(id);
                if (userModel == null)
                {
                    return HttpNotFound();
                }
                return View();
            }
            Session["Id"] = null;
            Session["Username"] = null;
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult ManagePassword([Bind(Include = "Id,Username,Password,Confirm,Hoten")] UserModel userModel, string OldPassword)
        {
            var user = db.userModels.Find(userModel.Id);
            if (GetSHA256(OldPassword) == user.Password)
            {
                user.Password = GetSHA256(userModel.Password);
                user.Confirm = GetSHA256(userModel.Confirm);
                if (Session["Auditing"] != null)
                {
                    if (user.Theodoi == true)
                    {
                        ActivityModel write = new ActivityModel();
                        string writeactivity = "Đã thay đổi mật khẩu";
                        int userid = (int)Session["Id"];
                        write.UserId = userid;
                        write.dateTime = DateTime.Now;
                        write.Log = writeactivity;
                        db.activityModels.Add(write);
                    }
                }
                db.SaveChanges();
                ViewBag.thongbao = "Đổi mật khẩu thành công";
                return RedirectToAction("ManageAccount");
            }
            else
            {
                ViewBag.error = "Mật khẩu cũ không đúng!";
            }
            return View();
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
            if (Session["Id"] != null && db.userModels.Find((int)Session["Id"]) != null)
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
            Session["Id"] = null;
            Session["Username"] = null;
            return RedirectToAction("Login");
        }

        // GET: News/Create
        public ActionResult Create()
        {
            if (Session["Id"] != null && db.userModels.Find((int)Session["Id"]) != null)
            {
                ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name");
                return View();
            }
            Session["Id"] = null;
            Session["Username"] = null;
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
                int id = (int)Session["Id"];
                var user = db.userModels.Find(id);
                if (Session["Auditing"] != null)
                {
                    if (user.Theodoi == true)
                    {
                        ActivityModel write = new ActivityModel();
                        string writeactivity = "Đã tạo mới một bài viết, id bài viết: " + newsModel.Id;
                        write.UserId = id;
                        write.dateTime = DateTime.Now;
                        write.Log = writeactivity;
                        db.activityModels.Add(write);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", newsModel.CategoryId);
            return View(newsModel);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Id"] != null && db.userModels.Find((int)Session["Id"]) != null)
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
            Session["Id"] = null;
            Session["Username"] = null;
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
                int id = (int)Session["Id"];
                var user = db.userModels.Find(id);
                if (Session["Auditing"] != null)
                {
                    if (user.Theodoi == true)
                    {
                        ActivityModel write = new ActivityModel();
                        string writeactivity = "Đã chỉnh sửa một bài viết, id bài viết: " + newsModel.Id;
                        write.UserId = id;
                        write.dateTime = DateTime.Now;
                        write.Log = writeactivity;
                        db.activityModels.Add(write);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", newsModel.CategoryId);
            return View(newsModel);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Id"] != null && db.userModels.Find((int)Session["Id"]) != null)
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
            Session["Id"] = null;
            Session["Username"] = null;
            return RedirectToAction("Login");
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NewsModel newsModel = db.newsModels.Find(id);
            db.newsModels.Remove(newsModel);
            int uid = (int)Session["Id"];
            var user = db.userModels.Find(uid);
            if (Session["Auditing"] != null)
            {
                if (user.Theodoi == true)
                {
                    ActivityModel write = new ActivityModel();
                    string writeactivity = "Đã xoá một bài viết, id bài viết: " + newsModel.Id;
                    write.UserId = uid;
                    write.dateTime = DateTime.Now;
                    write.Log = writeactivity;
                    db.activityModels.Add(write);
                }
            }
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
