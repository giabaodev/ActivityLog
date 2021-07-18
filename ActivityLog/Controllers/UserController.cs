using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ActivityLog.Models;
using PagedList;

namespace ActivityLog.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: User
        [Authorize]
        public ActionResult Index(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var user = from s in db.userModels
                           select s;
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                user = user.Where(s =>
                s.Hoten.ToUpper().Contains(searchString.ToUpper()) || s.Username.ToUpper().Contains(searchString.ToUpper())
                ||
                s.Id.ToString().Contains(searchString));
            }
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(user.OrderBy(s => s.Id).ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
        // GET: User/Details/5
        public ActionResult Details(int? id)
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
            return View(userModel);
        }
        [Authorize]
        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Username,Password,Confirm,Hoten")] UserModel userModel)
        {
            var checkUsername = db.userModels.SingleOrDefault(u => u.Username == userModel.Username);
            if (checkUsername == null)
            {
                userModel.Password = GetSHA256(userModel.Password);
                userModel.Confirm = GetSHA256(userModel.Confirm);
                db.Configuration.ValidateOnSaveEnabled = false;
                db.userModels.Add(userModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Tên tài khoản đã tồn tại!";
            }
            return View(userModel);
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
        [Authorize]
        // GET: User/Edit/5
        public ActionResult Edit(int? id)
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
            return View(userModel);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,Password,Confirm,Hoten")] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userModel);
        }
        [Authorize]
        // GET: User/Delete/5
        public ActionResult Delete(int? id)
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
            return View(userModel);
        }

        // POST: User/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserModel userModel = db.userModels.Find(id);
            db.userModels.Remove(userModel);
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
        public ActionResult Follow(int id)
        {
            var user = db.userModels.Find(id);
            user.Theodoi = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult UnFollow(int id)
        {
            var user = db.userModels.Find(id);
            user.Theodoi = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
