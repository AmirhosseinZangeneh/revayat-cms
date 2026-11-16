using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class PageGroupRepository : IPageGroupRepository
    {

        private RevayatContext db;

        public PageGroupRepository(RevayatContext context)
        {
            this.db = context;
        }
        public IEnumerable<PageGroup> GetAllGroups()
        {
            return db.PageGroups;
        }

        public PageGroup GetGroupById(int groupId)
        {
            return db.PageGroups.Find(groupId);
        }

        public bool InsertGroup(PageGroup pageGroup)
        {
            db.PageGroups.Add(pageGroup);
            return true;
        }

        public bool UpdateGroup(PageGroup pageGroup)
        {
            db.Entry(pageGroup).State=EntityState.Modified;
            return true;
        }

        public bool DeleteGroup(int groupId)
        {
            var group = GetGroupById(groupId);
            DeleteGroup(group);
            return true;
        }

        public bool DeleteGroup(PageGroup pageGroup)
        {
            db.Entry(pageGroup).State=EntityState.Deleted;
            return true;
        }

     

        public void save()
        {
            db.SaveChanges();
        }

        public IEnumerable<ShowGroupViewModel> GetGroupsForView()
        {
            return db.PageGroups.Select(g => new ShowGroupViewModel()
            {
                GroupID = g.GroupID,
                GroupTitle = g.GroupTitle,
                PageCount = g.Pages.Count
            });
        }


        public void Dispose()
        {
            db.Dispose();
        }
    }
}
