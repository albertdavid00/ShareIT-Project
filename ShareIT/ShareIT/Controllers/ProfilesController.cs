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

        [Authorize(Roles = "User,Admin")]
        public ActionResult MyProfile()
        {
            string uid = User.Identity.GetUserId();
            var prof = db.Profiles.Where(p => p.UserId == uid);
            ViewBag.myProfile = true;
            if (prof.Count() == 0)
            {
                return RedirectToAction("New", "Profiles");
            }
            else
            {
                int pid = prof.FirstOrDefault().ProfileId;   // Current user -> profile id
                return RedirectToAction("Show/" + pid.ToString());
            }
        }

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
        [Authorize(Roles = "User,Admin")]
        public ActionResult Show(int id)
        {
            Profile profile = db.Profiles.Find(id);
            var posts = from post in db.Posts
                        where post.UserId == profile.UserId
                        select post;
            ViewBag.Posts = posts;
            ViewBag.UserId = User.Identity.GetUserId();
            if(profile.DeletedByAdmin == true && profile.UserId == User.Identity.GetUserId())
            {
                profile.DeletedByAdmin = false;
                db.SaveChanges();
                TempData["warning"] = "Una din postarile tale o fost stearsa de catre admin din cauza continutului neadecvat";
                ViewBag.Warning = TempData["warning"];
            }
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            var currentUserProfile = db.Profiles.Where(p => p.UserId == currentUserId).FirstOrDefault();
            var user = profile.User;
            if (currentUserProfile.SentFriends.Contains(user))
            {
                ViewBag.sentRequest = true;
            }
            if (currentUserProfile.Friends.Contains(user)){
                ViewBag.friend = true;
            }
            if(currentUserId == profile.UserId)
            {
                ViewBag.sameUser = true;
            }
            return View(profile);
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult New()
        {
            if (TempData.ContainsKey("friend"))
            {
                ViewBag.Friend = TempData["friend"]; 
            }
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

        [Authorize(Roles = "User,Admin")]
        public ActionResult SendRequest(int id)
        {
            var profile = db.Profiles.Find(id);
            var user = profile.User;
            var currentUserId = User.Identity.GetUserId();
            var prof = db.Profiles.Where(p => p.UserId == currentUserId);
            if (prof.Count() == 0)
            {
                TempData["friend"] = "Creeaza-ti un profil pentru a adauga prieteni!";
                return RedirectToAction("New");
            }
            if(currentUserId == profile.UserId)
            {
                return RedirectToAction("Index");
            }
            var currentUserProfile = prof.FirstOrDefault();
            var currentUser = db.Users.Find(currentUserId);
            if (currentUserProfile.SentFriends.Contains(user)  || currentUserProfile.Friends.Contains(user))
            {
                return RedirectToAction("Index");
            }
            currentUserProfile.SentFriends.Add(user);
            profile.ReceivedFriends.Add(currentUser);
            db.SaveChanges();
            return RedirectToAction("Show/" + id.ToString());
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult FriendRequests(int id)
        {
            var profile = db.Profiles.Find(id);
            if (profile.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            List<ApplicationUser> friendRequests = new List<ApplicationUser>();
            foreach(var user in profile.ReceivedFriends)
            {
                friendRequests.Add(user);
            }
            ViewBag.profileId = profile.ProfileId;
            ViewBag.FriendRequests = friendRequests;
            ViewBag.Length = friendRequests.Count();
            return View(profile);
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult AddFriend(int id, int id2)
        {
/*            int pid = (int)this.RouteData.Values["id"];
            int i = (int)this.RouteData.Values["id2"];*/
            var profile = db.Profiles.Find(id);
            if (profile.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            List<ApplicationUser> friendRequests = new List<ApplicationUser>();
            foreach (var user in profile.ReceivedFriends)
            {
                friendRequests.Add(user);
            }
            for(int j = 0; j < friendRequests.Count(); j++)
            {
                var user = friendRequests[j];
                if(id2 == j)
                {
                    profile.Friends.Add(user);
                    var userId = user.Id;
                    var userProfile = db.Profiles.Where(p => p.UserId == userId).FirstOrDefault();
                    userProfile.Friends.Add(profile.User);

                    Friendship friendship = new Friendship();
                    friendship.User1_Id = profile.UserId;
                    friendship.User1 = profile.User;
                    friendship.User2_Id = userId;
                    friendship.User2 = user;
                    db.Friendships.Add(friendship);

                    profile.ReceivedFriends.Remove(user);
                    userProfile.SentFriends.Remove(profile.User);


                    db.SaveChanges();
                    break;
                }
            }
            return RedirectToAction("FriendRequests/" + id.ToString());
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult Friends(int id)
        {
            var profile = db.Profiles.Find(id);
            ViewBag.currentUser = User.Identity.GetUserId().ToString();
            ViewBag.UserId = profile.UserId.ToString();
           /* ViewBag.friends = profile.Friends;*/
            var userId = profile.UserId;
            var friendships1 = db.Friendships.Where(f => f.User1_Id == userId);
            List<ApplicationUser> friends1 = new List<ApplicationUser>();
            foreach(var friendship in friendships1)
            {
                friends1.Add(friendship.User2);
            }
            var friendships2 = db.Friendships.Where(f => f.User2_Id == userId);
            List<ApplicationUser> friends2 = new List<ApplicationUser>();
            foreach (var friendship in friendships2)
            {
                friends2.Add(friendship.User1);
            }
            var friends = friends1.Union(friends2);
            
            List<ApplicationUser> Friends = new List<ApplicationUser>();
            foreach(var friend in friends)
            {
                Friends.Add(friend);
            }
            ViewBag.friends = Friends;
            ViewBag.Length = friends.Count();
            ViewBag.profileId = profile.ProfileId;
            return View(profile);
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult DeclineFriend(int id, int id2)
        {
            var profile = db.Profiles.Find(id);
            if (profile.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            List<ApplicationUser> friendRequests = new List<ApplicationUser>();
            foreach (var user in profile.ReceivedFriends)
            {
                friendRequests.Add(user);
            }
            for (int j = 0; j < friendRequests.Count(); j++)
            {
                var user = friendRequests[j];
                if (id2 == j)
                {
                    var userId = user.Id;
                    var userProfile = db.Profiles.Where(p => p.UserId == userId).FirstOrDefault();

                    profile.ReceivedFriends.Remove(user);
                    userProfile.SentFriends.Remove(profile.User);

                    db.SaveChanges();
                    break;
                }
            }
            return RedirectToAction("FriendRequests/" + id.ToString());
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult DeleteFriend(int id, int id2)
        {

            var profile = db.Profiles.Find(id);
            if (profile.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            var userId = profile.UserId;
            var friendships1 = db.Friendships.Where(f => f.User1_Id == userId);
            List<ApplicationUser> friends1 = new List<ApplicationUser>();
            foreach (var friendship in friendships1)
            {
                friends1.Add(friendship.User2);
            }
            var friendships2 = db.Friendships.Where(f => f.User2_Id == userId);
            List<ApplicationUser> friends2 = new List<ApplicationUser>();
            foreach (var friendship in friendships2)
            {
                friends2.Add(friendship.User1);
            }
            var friends = friends1.Union(friends2);
            List<ApplicationUser> Friends = new List<ApplicationUser>();
            foreach(var friend in friends)
            {
                Friends.Add(friend);
            }

            for (int j = 0; j < Friends.Count(); j++)
            {
                var user = Friends[j];
                if (id2 == j)
                {
                    var user2Id = user.Id;
                    var userProfile = db.Profiles.Where(p => p.UserId == user2Id).FirstOrDefault();

                    var friendShips = db.Friendships.Where(f => (f.User1_Id == userId && f.User2_Id == user2Id) || (f.User2_Id == userId && f.User1_Id == user2Id));
                    foreach(var friendship in friendShips)
                    {
                        db.Friendships.Remove(friendship); 
                    }
                    db.SaveChanges();
                    break;
                }
            }
            return RedirectToAction("Friends/" + id.ToString());
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult JoinedGroups(int id)
        {
            var profile = db.Profiles.Find(id);
            var user = profile.User;
            ViewBag.joinedGroups = user.Groups;
            return View(profile);
        }
    }
}