using System.ComponentModel.DataAnnotations;

namespace Revayat.Web.Models
{
    public class CommentInput
    {
        [Required, StringLength(500)]
        public string Comment { get; set; }
    }
}