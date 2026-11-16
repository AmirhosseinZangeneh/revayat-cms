using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using Revayat.Web.Classes;

namespace Revayat.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        private IPageRepository pageRepository;
        private IPageGroupRepository pageGroupRepository;
        private RevayatContext db = new RevayatContext();


        public PagesController()
        {
            pageRepository=new PageRepository(db);
            pageGroupRepository=new PageGroupRepository(db);
        }

        // GET: Admin/Pages
        public ActionResult Index()
        {
            return View(pageRepository.GetAllPage());
        }

        // GET: Admin/Pages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Page page = db.Pages.Find(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            return View(page);
        }

        // GET: Admin/Pages/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(pageGroupRepository.GetAllGroups(), "GroupID", "GroupTitle");
            return View();
        }

        // POST: Admin/Pages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,Title,ShortDescription,Text,ShowInSlider,Tags")] Page page,HttpPostedFileBase imgUp)
        {
            if (!db.PageGroups.Any(g => g.GroupID == page.GroupID))
                ModelState.AddModelError("GroupID", "گروه معتبر نیست.");
            if (imgUp == null)
                ModelState.AddModelError("ImageName", "تصویر را انتخاب کنید.");

            if (ModelState.IsValid)
            {
                page.Visit = 0;
                page.CreateDate=DateTime.Now;

                string imageName;
                string imageError;
                var imageDirectory = Server.MapPath("~/PageImages");
                if (!PageImageStorage.TrySave(imgUp, imageDirectory, out imageName, out imageError))
                {
                    ModelState.AddModelError("ImageName", imageError);
                }
                else
                {
                    page.ImageName = imageName;
                    try
                    {
                        pageRepository.InsertPage(page);
                        pageRepository.Save();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        PageImageStorage.DeleteIfOwned(imageDirectory, imageName);
                        throw;
                    }
                }
            }

            ViewBag.GroupID = new SelectList(db.PageGroups, "GroupID", "GroupTitle", page.GroupID);
            return View(page);
        }

        // GET: Admin/Pages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Page page = pageRepository.GetPageById(id.Value);
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = new SelectList(pageGroupRepository.GetAllGroups(), "GroupID", "GroupTitle", page.GroupID);
            return View(page);
        }

        // POST: Admin/Pages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PageID,GroupID,Title,ShortDescription,Text,ShowInSlider,Tags")] Page page,HttpPostedFileBase imgUp)
        {
            var existing = pageRepository.GetPageById(page.PageID);
            if (existing == null)
                return HttpNotFound();
            if (!db.PageGroups.Any(g => g.GroupID == page.GroupID))
                ModelState.AddModelError("GroupID", "گروه معتبر نیست.");

            if (ModelState.IsValid)
            {
                var imageDirectory = Server.MapPath("~/PageImages");
                string newImageName = null;
                if (imgUp != null)
                {
                    string imageError;
                    if (!PageImageStorage.TrySave(imgUp, imageDirectory, out newImageName, out imageError))
                    {
                        ModelState.AddModelError("ImageName", imageError);
                    }
                }

                if (ModelState.IsValid)
                {
                    var oldImageName = existing.ImageName;
                    existing.GroupID = page.GroupID;
                    existing.Title = page.Title;
                    existing.ShortDescription = page.ShortDescription;
                    existing.Text = page.Text;
                    existing.Tags = page.Tags;
                    existing.ShowInSlider = page.ShowInSlider;
                    if (newImageName != null)
                        existing.ImageName = newImageName;
                    try
                    {
                        pageRepository.Save();
                    }
                    catch
                    {
                        PageImageStorage.DeleteIfOwned(imageDirectory, newImageName);
                        throw;
                    }

                    if (newImageName != null)
                        PageImageStorage.DeleteIfOwned(imageDirectory, oldImageName);
                    return RedirectToAction("Index");
                }
            }
            page.ImageName = existing.ImageName;
            ViewBag.GroupID = new SelectList(db.PageGroups, "GroupID", "GroupTitle", page.GroupID);
            return View(page);
        }

        // GET: Admin/Pages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Page page = pageRepository.GetPageById(id.Value);
            if (page == null)
            {
                return HttpNotFound();
            }
            return View(page);
        }

        // POST: Admin/Pages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var page = pageRepository.GetPageById(id);
            if (page == null)
                return HttpNotFound();
            var imageName = page.ImageName;
            pageRepository.DeletePage(page);
            pageRepository.Save();
            PageImageStorage.DeleteIfOwned(Server.MapPath("~/PageImages"), imageName);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pageGroupRepository.Dispose();
                pageRepository.Dispose();
                db.Dispose();
               
               
            }
            base.Dispose(disposing);
        }
    }
}
