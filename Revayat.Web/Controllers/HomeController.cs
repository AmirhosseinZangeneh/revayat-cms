using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace Revayat.Web.Controllers
{
    public class HomeController : Controller
    {
        RevayatContext db=new RevayatContext();
        private IPageRepository pageRepository;

        public HomeController()
        {
            pageRepository=new PageRepository(db);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Slider()
        {
            return PartialView(pageRepository.PagesInSlider());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
