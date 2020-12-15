using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShareIT.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        
        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual Post Post { get; set; }
    }
}