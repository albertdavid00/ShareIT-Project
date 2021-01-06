using Microsoft.AspNet.Identity;
using ShareIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareIT.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ShareIT.Models.ApplicationDbContext();


        // GET: Posts
        public ActionResult Index()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var userId = User.Identity.GetUserId();
            var groups = db.Groups;
            List<int> groupIds = new List<int>();
            foreach(var group in groups)
            {
                if (group.Users.Contains(currentUser))
                {
                    groupIds.Add(group.GroupId);
                }
            }
            var posts = db.Posts.Include("Profile").Where(p => (p.Group == null) || (groupIds.Contains(p.Group.GroupId)));

            List<Post> revPosts = new List<Post>();
            foreach (var post in posts)
            {
                revPosts.Add(post);
            }
            revPosts.Reverse();         // cea mai noua postare apare prima in news feed
            ViewBag.Posts = revPosts;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();

        }
        //SHOW
        public ActionResult Show(int id)
        {
            Post post = db.Posts.Find(id);
            ViewBag.UserId = User.Identity.GetUserId();
            return View(post);
        }
        //GET: New
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(int? id)
        {
            Post post = new Post();
            post.UserId = User.Identity.GetUserId();
            if(!(id is null))
            {
                post.Group = db.Groups.Find(id);
            }
            return View(post);
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(int? id, Post post)
        {
            if (!(id is null))
            {
                post.Group = db.Groups.Find(id);
            }
            post.Date = DateTime.Now;
            post.UserId = User.Identity.GetUserId();    // NICEEEEE
            try
            {
                if (ModelState.IsValid)
                {
                    db.Posts.Add(post);
                    db.SaveChanges();
                    TempData["message"] = "Postarea a fost adaugata";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(post);
                }
            }
            catch (Exception e)
            {
                return View(post);
            }
        }
        //EDIT
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            if (post.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(post);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei postari care nu va apartine";
                return RedirectToAction("Index");
            }
        }
        [HttpPut]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id, Post requestPost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Post post = db.Posts.Find(id);
                    if (post.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(post))
                        {
                            post.Title = requestPost.Title;
                            post.Content = requestPost.Content;
                            db.SaveChanges();
                            TempData["message"] = "Postarea a fost editata!";
                            return RedirectToAction("Index");
                        }
                        return View(requestPost);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei postari care nu va apartine";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(requestPost);
                }
            }
            catch (Exception e)
            {
                return View(requestPost);
            }

        }
        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            if (post.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                if (User.IsInRole("Admin"))
                {
                    var prof = db.Profiles.Where(p => p.UserId == post.UserId);
                    if (prof.Count() != 0)
                        prof.FirstOrDefault().DeletedByAdmin = true;
                }
                db.Posts.Remove(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o postare care nu va apartine";
                return RedirectToAction("Index");
            }
        }
    }
}