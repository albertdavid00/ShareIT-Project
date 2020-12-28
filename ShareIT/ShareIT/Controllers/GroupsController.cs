using Microsoft.AspNet.Identity;
using ShareIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareIT.Controllers
{
    public class GroupsController : Controller
    {
        private ApplicationDbContext db = new ShareIT.Models.ApplicationDbContext();
        // GET: Groups
        public ActionResult Index()
        {
            var groups = db.Groups;
            ViewBag.Groups = groups;
            ViewBag.CurrentUser = db.Users.Find(User.Identity.GetUserId());
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        public ActionResult Show(int id)
        {
            ViewBag.CurrentUser = db.Users.Find(User.Identity.GetUserId());
            Group grup = db.Groups.Find(id);
            var posts = from post in db.Posts.Include("Group")
                        where post.Group.GroupId == grup.GroupId
                        select post;
            ViewBag.Posts = posts;
            var users = grup.Users;
            ViewBag.Users = users;
            ViewBag.UserId = User.Identity.GetUserId();
            var owner = db.Users.Find(grup.CreatorId);
            ViewBag.OwnerName = owner.UserName;
            return View(grup);
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult New()
        {
            Group grup = new Group();
            grup.CreatorId = User.Identity.GetUserId();
            db.SaveChanges();
            return View(grup);
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Group grup)
        {
            grup.CreatorId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    /*var user = db.Users.Find(User.Identity.GetUserId());
                    grup.Users.Add(user);
                    user.Groups.Add(grup);*/
                    db.Groups.Add(grup);
                    db.SaveChanges();
/*                    var user = db.Users.Find(User.Identity.GetUserId());
                    grup.Users.Add(user);
                    user.Groups.Add(grup);
                    db.SaveChanges();*/
                    TempData["message"] = "Grupul a fost adaugat cu succes!";
                    return RedirectToAction("AddOwnerToGroup/" + grup.GroupId.ToString());
                }
                else
                {
                    return View(grup);
                }
            }
            catch (Exception e)
            {
                return View(grup);
            }
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            if(group.CreatorId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.Group = group;
                return View(group);
            }
            else
            {
                TempData["message"] = "Nu puteti modifica grupul.";
                return RedirectToAction("Index");
            }
        }
        [HttpPut]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id, Group requestGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Group group = db.Groups.Find(id);
                    if(group.CreatorId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if(TryUpdateModel(group))      
                        {
                            group = requestGroup;
                            db.SaveChanges();
                            TempData["message"] = "Grupul a fost editat";
                            return RedirectToAction("Index");
                        }
                        return View(requestGroup);
                    }
                    else
                    {
                        TempData["message"] = "Nu puteti modifica acest grup";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(requestGroup);
                }
            }
            catch(Exception e)
            {
                return View(requestGroup);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Group group = db.Groups.Find(id);
            if(group.CreatorId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Groups.Remove(group);
                db.SaveChanges();
                TempData["message"] = "Grupul a fost sters";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti grupul.";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult Join(int id)
        {
            Group group = db.Groups.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            var owner = db.Users.Find(group.CreatorId);
            if (User.Identity.GetUserId() != group.CreatorId && !group.Users.Contains(user))
            {
                group.Users.Add(user);
                user.Groups.Add(group);
                // de modificat
                /*if (!group.Users.Contains(owner))
                {
                    group.Users.Add(owner);
                    owner.Groups.Add(group);
                }*/
                db.SaveChanges();
                TempData["message"] = "Ai intrat in grup!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Deja faci parte din acest grup!";
                return RedirectToAction("Index");
            }
        }
        
        [Authorize(Roles = "User,Admin")]
        public ActionResult Leave(int id)
        {
            Group group = db.Groups.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (group.Users.Contains(user) && User.Identity.GetUserId() != group.CreatorId)
            {
                group.Users.Remove(user);
                user.Groups.Remove(group);
                db.SaveChanges();
                TempData["message"] = "Ai parasit grupul!";
                return RedirectToAction("Index");
            }
            else
            {
                if (User.Identity.GetUserId() == group.CreatorId)
                    TempData["message"] = "Nu poti parasi un grup pe care l-ai creat!";
                else
                    TempData["message"] = "Nu faci parte din grup pentru a il parasi.";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult Members(int id)
        {
            Group group = db.Groups.Find(id);
            var members = group.Users;
            ViewBag.Owner = db.Users.Find(group.CreatorId).UserName;
            ViewBag.Members = members;
            return View(group);
        }
        
        [Authorize(Roles = "User,Admin")]
        public ActionResult AddOwnerToGroup(int id)
        {
            Group group = db.Groups.Find(id);
            var owner = db.Users.Find(group.CreatorId);
            if (User.Identity.GetUserId() == group.CreatorId)
            {
                if (!group.Users.Contains(owner))
                {
                    group.Users.Add(owner);
                    owner.Groups.Add(group);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
       /* [Authorize(Roles = "User,Admin")]
        public ActionResult DeleteMember(string name, int id)
        {
            Group group = db.Groups.Find(id);
            var currentUser = User.Identity.GetUserId();
            if (currentUser == group.CreatorId)
            {
                var deleteUser = from u in db.Users
                                 where u.UserName == name
                                 select u;
                group.Users.Remove(deleteUser.FirstOrDefault());
                deleteUser.FirstOrDefault().Groups.Remove(group);
                db.SaveChanges();
            }
            return RedirectToAction("Show/" + group.GroupId.ToString());
        }*/
    }
}