using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace Revayat.Web.Controllers
{
    public class SearchController : Controller
    {
        private IPageRepository pageRepository;
        RevayatContext db=new RevayatContext();

        public SearchController()
        {
            pageRepository=new PageRepository(db);
        }
        // GET: Search
        public ActionResult Index(string q)
        {
            ViewBag.Name = q;
            return View(string.IsNullOrWhiteSpace(q) ? Enumerable.Empty<Page>() : pageRepository.SearchPage(q.Trim()));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
