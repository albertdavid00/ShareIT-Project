using ShareIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppContext = ShareIT.Models.AppContext;

namespace ShareIT.Controllers
{
    public class ProfilesController : Controller
    {
        private AppContext db = new ShareIT.Models.AppContext();
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
            
            return View(profile);
        }

        public ActionResult New()
        {
            Profile profile = new Profile();

            return View(profile);
        }

        [HttpPost]
        public ActionResult New(Profile profile)
        {
            profile.SignUpDate = DateTime.Now;
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

        public ActionResult Edit(int id)
        {
            Profile profile = db.Profiles.Find(id);
            ViewBag.Profile = profile;
            return View(profile);
        }

        [HttpPut]
        public ActionResult Edit(int id, Profile requestProfile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Profile profile = db.Profiles.Find(id);
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
                    return View(requestProfile);
                }
            }
            catch (Exception e)
            {
                return View(requestProfile);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Profile profile = db.Profiles.Find(id);
            db.Profiles.Remove(profile);
            db.SaveChanges();
            TempData["message"] = "Profilul a fost sters!";
            return RedirectToAction("Index");
        }
    }
}