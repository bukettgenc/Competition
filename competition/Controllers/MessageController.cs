using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using competition.Models;

namespace competition.Controllers
{
    public class MessageController : Controller
    {
        // GET: Message
        public ActionResult MyMessages(int? id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ViewMessage(int? id, int? profileNu)
        {

            ViewBag.Id = id;
            ViewBag.profileNu = profileNu;
            return View();
        }
        public JsonResult ViewMessages(int sayi, int mesajAtan, FormCollection form)
        {
            DatabaseContext db = new DatabaseContext();

            string mesajım = Request.Form["mesaj"];
            MessagesBox message = new MessagesBox();
            message.ProfileNu = sayi;
            message.MessageSender = (int)mesajAtan;
            message.Message = mesajım;
            db.MessagesBoxes.Add(message);
            db.SaveChanges();

            List<MessagesBox> Listem = db.MessagesBoxes.ToList();
            MessagesBox messagesBox = Listem.LastOrDefault();

            return Json(messagesBox, JsonRequestBehavior.AllowGet);
        }

    }
}