using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DataLayer;
using Revayat.Web.Areas.Admin.Models;

namespace Revayat.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly RevayatContext db = new RevayatContext();

        public ActionResult Index()
        {
            var today = DateTime.Today;
            var start = today.AddDays(-6);
            var end = today.AddDays(1);
            var dates = db.Pages.Where(p => p.CreateDate >= start && p.CreateDate < end)
                .Select(p => p.CreateDate).ToList();
            var model = new DashboardViewModel
            {
                ArticleCount = db.Pages.Count(),
                GroupCount = db.PageGroups.Count(),
                CommentCount = db.PageComments.Count(),
                TotalVisits = db.Pages.Sum(p => (int?)p.Visit) ?? 0,
                DailyArticles = Enumerable.Range(0, 7)
                    .Select(i => dates.Count(date => date.Date == start.AddDays(i))).ToArray(),
                DayLabels = Enumerable.Range(0, 7)
                    .Select(i => start.AddDays(i).ToShamsi()).ToArray(),
                RecentArticles = db.Pages.Include(p => p.PageGroup)
                    .OrderByDescending(p => p.CreateDate).Take(5).ToList()
            };
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}