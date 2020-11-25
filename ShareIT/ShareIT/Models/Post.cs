using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareIT.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(40, ErrorMessage = "Titlul nu poate avea mai mult de 40 de caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Continutul postarii este obligatoriu")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        
        // user_id

        public virtual Profile Profile { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}