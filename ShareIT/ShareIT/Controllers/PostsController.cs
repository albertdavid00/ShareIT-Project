using ShareIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppContext = ShareIT.Models.AppContext;

namespace ShareIT.Controllers
{
    public class PostsController : Controller
    {
        private AppContext db = new ShareIT.Models.AppContext();


        // GET: Posts
        public ActionResult Index()
        {
            var posts = db.Posts.Include("Profile");
            ViewBag.Posts = posts;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        //SHOW
        public ActionResult Show(int id)
        {
            Post posts = db.Posts.Find(id);
            ViewBag.Posts = posts;
            //ViewBag.Articles = posts;
            return View();
        }
        //GET: New
        public ActionResult New()
        {
            Post post = new Post();
            return View(post);
        }
        [HttpPost]
        public ActionResult New(Post post)
        {
            post.Date = DateTime.Now;
            try
            {
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost adaugat cu succes";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(post);
            }
        }
        //EDIT

        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);

            return View(post);
        }
        [HttpPut]
        public ActionResult Edit(int id, Post requestPost)
        {
            try
            {
                Post post = db.Posts.Find(id);
                if (TryUpdateModel(post))
                {
                    post.Title = requestPost.Title;
                    post.Content = requestPost.Content;
                    db.SaveChanges();
                    TempData["message"] = "Articolul a fost editat!";
                    return RedirectToAction("Index");

                }
                return View(requestPost);
            }
            catch (Exception e)
            {
                return View(requestPost);
            }

        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}