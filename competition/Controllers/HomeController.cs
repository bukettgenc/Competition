using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using competition.Models;
using FormCollection = System.Web.Mvc.FormCollection;

namespace competition.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();

        public ActionResult Index(int? id)
        {
            if (id != null && id != 0)
            {
                User user = db.Users.Where(i => i.ProfileNu == id).SingleOrDefault();
                user.IsOnline = false;
                db.SaveChanges();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            String indexName = Request.Form["indexName"];
            String indexPassword = Request.Form["indexPassword"];
            string directing = "";
            string controller = "";
            var userList = db.Users.ToList();
            int i = 0;
            int profileNu = 0;
            foreach (var user in userList)
            {
                if (user.Username == indexName && user.Password == indexPassword)
                {
                    directing = "Profile";
                    controller = "User";
                    user.IsOnline = true;
                    profileNu = user.ProfileNu;
                    db.SaveChanges();
                    i++;
                }
                else
                {
                    if (i == 0)
                    {
                        directing = "Index";
                        controller = "Home";
                        profileNu = 0;

                    }
                }

            }

            if (i == 0)
            {
              MessageBox.Show("KULLANICI ADINIZ VEYA ŞİFRENİZ YANLIŞ ");
            }
            return RedirectToAction(directing, controller, new { @id = profileNu });
        }

        public ActionResult SignUp()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SignUp(FormCollection form, HttpPostedFileBase file)
        {
            var numberList = db.Users.ToList().LastOrDefault();
            var accountControlList = db.Users.ToList();
            string signUpName = Request.Form["signUpName"];
            string signUpEmail = Request.Form["signUpEmail"];
            string signUpPassword = Request.Form["signUpPassword"];
            string reSignUpPassword = Request.Form["reSignUpPassword"];
            int flag = 0;
            foreach (var control in accountControlList)
            {
                if (control.Username != signUpName && control.Mail != signUpEmail)
                {
                    flag = 0;
                }
                else
                {
                    flag = 1;
                    goto Etiket;
                }
            }
        Etiket:
            if (flag == 0)
            {
                if (signUpPassword == reSignUpPassword)
                {
                    User user = new User();
                    user.Username = signUpName;
                    user.Mail = signUpEmail;
                    user.Password = signUpPassword;
                    if (file != null && file.ContentLength > 0)
                        try
                        {
                            string profilePht = RandomString(10);
                            string path = Path.Combine(Server.MapPath("~/ProfilePhoto/"),
                                Path.GetFileName(profilePht+".png"));
                            file.SaveAs(path);
                            user.ProfileImage = "~/ProfilePhoto/" + profilePht + ".png";
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    else
                    {
                        user.ProfileImage = "~/images/user.jpg";
                    }
                    user.HomepageNu = numberList.HomepageNu + 1;
                    user.ProfileNu = numberList.ProfileNu + 1;
                    user.IsOnline = false;
                    db.Users.Add(user);
                    db.SaveChanges();
                    MessageBox.Show("KAYIT BAŞARILI");
                }
                else
                {
                    MessageBox.Show("ŞİFRELERİNİZ EŞLEŞMEDİ,TEKRAR DENEYİNİZ");
                }
            }
            else if (flag == 1)
            { MessageBox.Show("KULLANICI ADINIZ VEYA MAİL ADRESİNİZ DAHA ÖNCE KULLANILMIŞ,KAYIT BAŞARISIZ ");

            }
            return View();
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ActionResult IForgotMyPassword()
        {

            return View();
        }
        [HttpPost]
        public ActionResult IForgotMyPassword(FormCollection form)
        {
            var userList = db.Users.ToList();

            String randomPassword = RandomString(6);
            string forgotMail = Request.Form["forgotMail"];
            var forgotPassworduser = db.Users.Where(x => x.Mail == forgotMail).SingleOrDefault();

            SmtpClient sc = new SmtpClient();
            sc.Port = 587;
            sc.Host = "smtp.gmail.com";
            sc.EnableSsl = true;
            sc.Credentials = new NetworkCredential("bktgnc06@gmail.com", "golge0210");
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("bktgnc06@gmail.com", "competition");

            mail.To.Add(forgotMail);

            mail.Subject = "ŞİFRENİZ DEĞİŞTİRİLDİ!";
            mail.IsBodyHtml = true;
            mail.Body = "Yeni şifreniz:" + randomPassword;
            foreach (var user in userList)
            {
                if (user.Mail == forgotMail)
                {
                    sc.Send(mail);
                    forgotPassworduser.Password = randomPassword;
                    db.SaveChanges();
                }
            }


            return View();
        }
    }
}