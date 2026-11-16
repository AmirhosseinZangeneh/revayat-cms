using System.Collections.Generic;
using DataLayer;

namespace Revayat.Web.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int ArticleCount { get; set; }
        public int GroupCount { get; set; }
        public int CommentCount { get; set; }
        public int TotalVisits { get; set; }
        public int[] DailyArticles { get; set; }
        public string[] DayLabels { get; set; }
        public IList<Page> RecentArticles { get; set; }
    }
}