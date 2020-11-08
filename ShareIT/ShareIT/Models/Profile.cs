using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShareIT.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }
        [Required]
        public string ProfileName { get; set; }
        [Required]

        public string ProfileDescription { get; set; }
        [Required]
        public DateTime SignUpDate { get; set; }
        //user_id
        public virtual ICollection<Post> Posts { get; set; }
    }
}