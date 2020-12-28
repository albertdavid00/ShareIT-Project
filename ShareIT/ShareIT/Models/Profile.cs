using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShareIT.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }
     
        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(30, ErrorMessage = "Numele nu poate avea mai mult de 30 de caractere")]
        public string ProfileName { get; set; }

        [Required(ErrorMessage = "Descrierea profilului este obligatorie")]
        [DataType(DataType.MultilineText)]
        public string ProfileDescription { get; set; }
        [Required]
        public DateTime SignUpDate { get; set; }
        public bool PrivateProfile { get; set; }
        [Display(Name = "ProfilePicture")]
        public byte[] ProfilePicture { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public bool DeletedByAdmin { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}