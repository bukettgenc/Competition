using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using competition.HavaDurumuServis;
using competition.Models;
using competition.ServisUser;
using User = competition.Models.User;

namespace competition.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        DatabaseContext db = new DatabaseContext();
        WebService1SoapClient service = new WebService1SoapClient();
        HavaDurumuSoapClient havaDurumuServis=new HavaDurumuSoapClient();

        public static string EncodeBase64(string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public static string DecodeBase64(string value)
        {
            var valueBytes = System.Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ActionResult deneme()
        {
        
            return View();
        }
        public ActionResult CompetitionHomePage(int id, int? categoryId)
        {
            ViewBag.id = id;
            if (categoryId == null)
            {
                categoryId = 0;
            }
            ViewBag.categoryId = categoryId;
            return View();
        }
        public ActionResult FriendProfile(int id, int profileNu)
        {
            bool isFriend = false;
            bool isBlocked = false;
            var followerList = db.Followers.ToList();
            var blockedList = db.Blockeds.ToList();
            foreach (var follower in followerList)
            {
                if (follower.ProfileNu == id && follower.FriendProfileNu == profileNu)
                {
                    isFriend = true;
                }
            }  
            foreach (var blocked in blockedList)
            {
                if (blocked.ProfileNu == id && blocked.BlockedProfileNu == profileNu)
                {
                    isBlocked = true;
                }
            }
            ViewBag.isFriend = isFriend;
            ViewBag.isBlocked = isBlocked;
            ViewBag.id = id;
            ViewBag.profileNu = profileNu;
            return View();
        }
        public ActionResult ResultPage(int id)
        {
            /*int id, int profileNu*/
            //bool isFriend = false;
            //var followerList = db.Followers.ToList();
            //foreach (var item in followerList)
            //{
            //    if (item.ProfileNu == id && item.FriendProfileNu == profileNu)
            //    {
            //        isFriend = true;
            //    }

            //}

            //ViewBag.isFriend = isFriend;
            ViewBag.id = id;
            //ViewBag.profileNu = profileNu;
            return View();
        }

        public ActionResult Profile(int id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult Profile(FormCollection form, HttpPostedFileBase file, int id)
        {
            DateTime nowDate = DateTime.Now;
            ViewBag.Id = id;
            ProfilePost post = new ProfilePost();
            post.NumberofLikes = 0;
            post.ProfilePosts = Request.Form["shareName"];
            post.Date = nowDate;
            if (Request.Form["radioName"] != null)
            {
                post.CategoryId = Int32.Parse(Request.Form["radioName"]);
            }
            else
            {
                post.CategoryId = 0;
            }


            post.ProfileNu = id;
            if (Int32.Parse(Request.Form["selectName"]) == 1)
            {
                post.ForTheCompetition = false;
            }
            else if (Int32.Parse(Request.Form["selectName"]) == 2)
            {
                post.ForTheCompetition = true;
            }

            if (file != null && file.ContentLength > 0)
                try
                {
                    String random = RandomString(10);

                    string path = Path.Combine(Server.MapPath("~/Upload/"),
                    Path.GetFileName(random + ".mp4"));
                    file.SaveAs(path);
                    post.VideoPosts = "~/Upload/" + random + ".mp4";
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            db.ProfilePosts.Add(post);
            db.SaveChanges();
            return View();
        }
        [HttpPost]
        public JsonResult Share(int sayi, ProfilePost pp)
        {
            ProfilePost post = new ProfilePost();
            post.ProfilePosts = pp.ProfilePosts;
            post.NumberofLikes = 0;
            post.ProfileNu = sayi;
            db.ProfilePosts.Add(post);
            db.SaveChanges();

            List<ProfilePost> profileList = db.ProfilePosts.ToList();
            ProfilePost posts = profileList.LastOrDefault();

            return Json(posts, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Homepage(int id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public JsonResult Like(int id, int tiklananId)
        {
            var allList = new List<ProfilePost>();
            var begeniList = db.PostLikeses.ToList();
            var TakipEdilen = db.Followers.Where(x => x.ProfileNu == id).ToList().OrderByDescending(x => x.Id);
            foreach (var item in TakipEdilen)
            {
                var PageList = db.ProfilePosts.Where(x => x.ProfileNu == item.FriendProfileNu).ToList().OrderByDescending(x => x.Id);
                allList.AddRange(PageList);

            }

            foreach (var item in allList)
            {
                if (item.Id == tiklananId)
                {
                    PostLikes postLikes = new PostLikes();

                    int degisken;
                    if (db.PostLikeses.Where(x => x.PostNu == tiklananId && x.LikeProfileNu == id).ToList().Count == 0)
                    {
                        degisken = item.NumberofLikes + 1;
                        item.NumberofLikes = degisken;
                        postLikes.PostNu = tiklananId;
                        postLikes.LikeProfileNu = id;
                        db.PostLikeses.Add(postLikes);

                    }
                    else
                    {
                        degisken = item.NumberofLikes - 1;
                        item.NumberofLikes = degisken;
                        foreach (var begeni in begeniList)
                        {
                            if (begeni.LikeProfileNu == id && begeni.PostNu == tiklananId)
                            {
                                PostLikes deptDelete = db.PostLikeses.Find(begeni.Id);
                                db.PostLikeses.Remove(deptDelete);

                            }
                        }
                    }
                    db.SaveChanges();

                }
            }

            int i = 0;
            List<ProfilePost> Listem = db.ProfilePosts.ToList();
            ProfilePost anasayfaYazilari = Listem.FirstOrDefault(x => x.Id == tiklananId);
            string buton_adi = "";
            int flagg = 0;
            //item id aslında tıklananın idsi


            foreach (var begeni in begeniList)
            {
                //giriş yapılan
                if (tiklananId == begeni.PostNu && begeni.LikeProfileNu == id)
                {
                    i = 1;
                    flagg = 1;
                }
                else
                {
                    if (flagg == 0)
                    {
                        i = 0;
                    }
                }
            }


            if (i == 0)
            {
                buton_adi = "Geri Al";
            }
            else
            {
                buton_adi = "Beğen";
            }
            anasayfaYazilari.ProfilePosts = buton_adi;//profil yazıları yerine yeni bi sütun açılacak.

            return Json(anasayfaYazilari, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Followers(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult Following(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult AddFriends(int id, int profileNu)
        {
            var followList = db.Followers.ToList();
            bool Exist = false;
            int ExistId = 0;
            foreach (var follow in followList)
            {
                if (follow.ProfileNu == id && follow.FriendProfileNu == profileNu)
                {
                    Exist = true;
                    ExistId = follow.Id;
                }
            }
            if (Exist == true)
            {
                Follower deptDelete = db.Followers.Find(ExistId);
                db.Followers.Remove(deptDelete);
            }
            else
            {

                Follower follower = new Follower();
                follower.ProfileNu = id;
                follower.FriendProfileNu = profileNu;
                db.Followers.Add(follower);

            }

      
            db.SaveChanges();
            return RedirectToAction("FriendProfile", "User", new { id = id, profileNu = profileNu });

        }
        public ActionResult Block(int id, int profileNu)
        {
           
            var followList = db.Followers.ToList();
            var blockedList = db.Blockeds.ToList();
            int blockedId = 0, blockedId2 = 0, blockedIdDel = 0;
            bool Exist = false;


            foreach (var follow in followList)
            {
                if (follow.ProfileNu == id && follow.FriendProfileNu == profileNu)
                {

                    blockedId = follow.Id;
                }
                else if (follow.ProfileNu == profileNu && follow.FriendProfileNu == id)
                {
                    blockedId2 = follow.Id;

                }
            }
      
            foreach (var block in blockedList)
            {
                if (block.ProfileNu == id && block.BlockedProfileNu == profileNu)
                {
                    Exist = true;
                    blockedIdDel = block.Id;

                }
            }
            if (Exist == true)
            {
                Blocked deleteBlock = db.Blockeds.Find(blockedIdDel);
                db.Blockeds.Remove(deleteBlock);
            }
            else
            {
                if (blockedId!=0)
                {
                    Follower delete = db.Followers.Find(blockedId);
                    db.Followers.Remove(delete);
                }
                if (blockedId2 != 0)
                {
                    Follower delete2 = db.Followers.Find(blockedId2);
                    db.Followers.Remove(delete2);
                }
              
                Blocked blocked = new Blocked();
                blocked.ProfileNu = id;
                blocked.BlockedProfileNu = profileNu;
                db.Blockeds.Add(blocked);

            }
            db.SaveChanges();
            return RedirectToAction("FriendProfile", "User", new { id = id, profileNu = profileNu });
            }
    }
}