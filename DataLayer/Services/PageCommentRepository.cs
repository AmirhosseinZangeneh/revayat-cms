using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
   public class PageCommentRepository:IPageCommentRepository
   {
       private RevayatContext db;

       public PageCommentRepository(RevayatContext context)
       {
           db = context;
       }
       public IEnumerable<PageComment> GetCommentByNewsId(int pageId)
       {
           return db.PageComments.Where(c => c.PageID == pageId);
       }

       public bool AddComment(PageComment comment)
       {
           db.PageComments.Add(comment);
           db.SaveChanges();
           return true;
       }
    }
}
