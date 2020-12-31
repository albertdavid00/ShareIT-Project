using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShareIT.Models
{
    public class Friendship
    {
        public int FriendshipId { get; set; }
        [ForeignKey("User1")]
        public string User1_Id { get; set; }
        public virtual ApplicationUser User1 { get; set; }

        [ForeignKey("User2")]
        public string User2_Id { get; set; }
        public virtual ApplicationUser User2 { get; set; }

    }
}