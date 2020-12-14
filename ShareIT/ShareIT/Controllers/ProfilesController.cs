using Microsoft.AspNet.Identity;
using ShareIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareIT.Controllers
{
    public class ProfilesController : Controller
    {
        private ApplicationDbContext db = new ShareIT.Models.ApplicationDbContext();
        // GET: Profiles
        public ActionResult Index()
        {
            var profiles = db.Profiles;
            ViewBag.Profiles = profiles;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        public ActionResult Show(int id)
        {
            Profile profile = db.Profiles.Find(id);
            var posts = from post in db.Posts
                        where post.UserId == profile.UserId
                        select post;
            ViewBag.Posts = posts;
            ViewBag.UserId = User.Identity.GetUserId();
            return View(profile);
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult New()
        {
            Profile profile = new Profile();
            profile.UserId = User.Identity.GetUserId();
            return View(profile);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Profile profile)
        {
            profile.SignUpDate = DateTime.Now;
            profile.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost adaugat cu succes!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(profile);
                }
            }
            catch (Exception e)
            {
                return View(profile);
            }
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            Profile profile = db.Profiles.Find(id);
            if (profile.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.Profile = profile;
                return View(profile);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id, Profile requestProfile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Profile profile = db.Profiles.Find(id);
                    if (profile.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(profile))
                        {
                            profile = requestProfile;
                            /*profile.ProfileName = requestProfile.ProfileName;
                            profile.ProfileDescription = requestProfile.ProfileDescription;
                            profile.SignUpDate = requestProfile.SignUpDate;
                            profile.PrivateProfile = requestProfile.PrivateProfile;*/
                            db.SaveChanges();
                            TempData["message"] = "Profilul a fost editat!";
                            return RedirectToAction("Index");
                        }
                        return View(requestProfile);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(requestProfile);
                }
            }
            catch (Exception e)
            {
                return View(requestProfile);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Profile profile = db.Profiles.Find(id);
            if (profile.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Profiles.Remove(profile);
                db.SaveChanges();
                TempData["message"] = "Profilul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un profil care nu va apartine";
                return RedirectToAction("Index");
            }
        }
    }
}