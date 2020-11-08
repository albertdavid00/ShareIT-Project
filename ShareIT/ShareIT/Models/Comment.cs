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
        [Required]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int PostId { get; set; }
        // user_id
        public virtual Post Post { get; set; }
    }
}