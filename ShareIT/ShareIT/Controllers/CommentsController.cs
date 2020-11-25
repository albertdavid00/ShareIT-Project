using System;
using ShareIT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppContext = ShareIT.Models.AppContext;

namespace ShareIT.Controllers
{
    public class CommentsController : Controller
    {
        private AppContext db = new ShareIT.Models.AppContext();

        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            db.Comments.Remove(comm);
            db.SaveChanges();
            return Redirect("/Posts/Show/" + comm.PostId);
        }

        [HttpPost]
        public ActionResult New(Comment comm)
        {
            comm.Date = DateTime.Now;
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

        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            //ViewBag.Comment = comm;
            return View(comm);
        }

        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Comment comm = db.Comments.Find(id);
                    if (TryUpdateModel(comm))
                    {
                        comm.Content = requestComment.Content;
                        db.SaveChanges();
                    }
                    return Redirect("/Posts/Show/" + comm.PostId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            catch (Exception e)
            {
                return View();
            }

        }


    }
}