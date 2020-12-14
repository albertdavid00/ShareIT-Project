using System;
using ShareIT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ShareIT.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ShareIT.Models.ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (!User.IsInRole("Admin") && !User.IsInRole("User"))
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Posts");      // DE CEEEEEEEE
            }
            else
            {
                if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    db.Comments.Remove(comm);
                    db.SaveChanges();
                    return Redirect("/Posts/Show/" + comm.PostId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                    return RedirectToAction("Index", "Posts");
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Comment comm)
        {
            comm.Date = DateTime.Now;
            comm.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comm);
                    db.SaveChanges();
                    return Redirect("/Posts/Show/" + comm.PostId);
                }
                else
                {
                    return Redirect("/Posts/Show/" + comm.PostId);
                }
            }

            catch (Exception e)
            {
                return Redirect("/Posts/Show/" + comm.PostId);
            }

        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Posts");
            }
        }

        [HttpPut]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comm = db.Comments.Find(id);

                if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(comm))
                    {
                        comm.Content = requestComment.Content;
                        db.SaveChanges();
                    }
                    return Redirect("/Posts/Show/" + comm.PostId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                    return RedirectToAction("Index", "Posts");
                }
            }
            catch (Exception e)
            {
                return View();
            }

        }


    }
}