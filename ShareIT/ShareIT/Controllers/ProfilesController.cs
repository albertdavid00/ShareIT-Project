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
        private int _perPage = 100;

        // GET: Profiles
        public ActionResult Index()
        {
            string uid = User.Identity.GetUserId();     // afisam butonul cu adauga profil daca nu are profil user-ul
            var prof = from p in db.Profiles
                           where p.UserId == uid        //
                           select p;
            ViewBag.NoProfile = prof.Count();           // 


            var search = "";
            var profiles = db.Profiles.OrderBy(p => p.SignUpDate);
            if(Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                //Search in profiles (ProfileName)
                List<int> ProfileIds = db.Profiles.Where(at => at.ProfileName.Contains(search)).Select(p => p.ProfileId).ToList();
                profiles = db.Profiles.Where(profile => ProfileIds.Contains(profile.ProfileId)).OrderBy(p => p.SignUpDate);
            }
            var totalItems = profiles.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }
            var paginatedProfiles = profiles.Skip(offset).Take(this._perPage);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Profiles = paginatedProfiles;
            ViewBag.SearchString = search;
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
            string uid = User.Identity.GetUserId();
            var profiles = from p in db.Profiles
                           where p.UserId == uid
                           select p;
            ViewBag.NoProfile = profiles.Count();
            if (profiles.Count() == 0)
                return View(profile);
            else 
                return RedirectToAction("Index", "Profiles");
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