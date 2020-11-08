﻿using System;
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
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        // user_id

        public virtual Profile Profile { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public IEnumerable<SelectListItem> Categ { get; set; }
    }
}