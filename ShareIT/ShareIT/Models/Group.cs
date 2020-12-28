using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShareIT.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(30, ErrorMessage = "Numele nu poate avea mai mult de 30 de caractere")]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        public string GroupDescription { get; set; }
        public string CreatorId { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}