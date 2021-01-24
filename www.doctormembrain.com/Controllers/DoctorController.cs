using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using www.doctormembrain.com.Extensions;
using www.doctormembrain.com.Models;
using www.doctormembrain.com.SharedClasses;

namespace www.doctormembrain.com.Controllers
{

    public class DoctorController : Controller
    {
        private IHttpContextAccessor httpcon;
        private Session session;
        StorieHandler sh;
        Statistics stats = new Statistics();

        protected UserManager<IdentityUser> UserManager { get; set; }

        public DoctorController(UserManager<IdentityUser> userManager) 
        {
            sh = new StorieHandler();
            this.UserManager = userManager;

            httpcon = new HttpContextAccessor();
            session = new Session(httpcon);
            Access access = new Access(Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(session.AccessGuid))
                session.AccessGuid = access.e.Guid;
        }

        private Task<IdentityUser> GetCurrentUserAsync()
        {
            return this.UserManager.GetUserAsync(HttpContext.User);
        }

        //[Route("{**any}")]
        [AllowAnonymous]
        public IActionResult CatchAll()
        {
            try
            {
                return NotFound(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }

        public async Task<ActionResult> UserProfile()
        {
            try
            {
                IdentityUser iuser = await GetCurrentUserAsync();
                IList<string> role_names = UserManager.GetRolesAsync(iuser).Result;
                string role_name = role_names.Contains("Administrator") ? "Administrator" : "";

                if (role_name == "Administrator")
                    return RedirectToRoute("admin_control_panel");

                return RedirectToRoute("welcome");
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }

        //[Route("admin_reset")]
        public IActionResult AdminControlPanel()
        {
            try
            {
                return View("AdminControlPanel");// StatusCodes.Status200OK;
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }
        [HttpPost]
        public int AdminControlPanelPost(string pwd)
        {
            try
            {
                string ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
                Stats stats_res = stats.GetStatistics(ip);
                string ip_str = " [" + stats_res.ip + "]";

                string msg;
                if (stats_res.ip == Settings.Basic.IP() || Check.Generel.IsAdmin(ip))
                    msg = "Reset! - Admin" + ip_str + " [" + stats_res.users_per_day + "]";
                else
                    msg = "Reset(Ugyldig)! - URL" + ip_str + " [" + stats_res.users_per_day + "]";

                string subject = msg;
                string body = "IP: " + stats_res.ip;
                Admin.Notification.Run(Settings.Basic.EMAIL_MAIL(), Settings.Basic.EMAIL_MAIL(), Settings.Basic.EMAIL_MAIL(), subject, body);


                if (pwd == "asDf1234" && (stats_res.ip == Settings.Basic.IP() || Check.Generel.IsAdmin(ip)))
                {
                    new StorieHandler().Reset();
                    return StatusCodes.Status200OK;
                }
                return StatusCodes.Status401Unauthorized;
            }
            catch (Exception e)
            {
                return StatusCodes.Status400BadRequest;
            }
        }

        [AllowAnonymous]
        public IActionResult Wiki(int? wiki_id) 
        {
            try
            {
                doctormembraincomContext db = new doctormembraincomContext();
                Wiki w;
                if (wiki_id != null)
                {
                    w = db.Wiki
                        .Include(x => x.Chapter)
                        .Where(x => x.Id == wiki_id)
                        .FirstOrDefault();
                }
                else
                {
                    w = db.Wiki
                        .Include(x => x.Chapter)
                        .FirstOrDefault();
                }
                if (w != null)
                {
                    w.Title = StringHelper.SearchStringForTag(w.Title, "a");
                    w.Intro = StringHelper.SearchStringForTag(w.Intro, "a");
                    foreach(Chapter ch in w.Chapter)
                    {
                        ch.Title = StringHelper.SearchStringForTag(ch.Title, "a");
                        ch.Text = StringHelper.SearchStringForTag(ch.Text, "a");
                    }
                    w.Chapter = w.Chapter.OrderBy(x => x.Priority).ToList();
                    return View("Wiki", w);
                }
                return RedirectToRoute("welcome");
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }

        public IActionResult CreateWiki(int? wiki_id = null)
        {
            try
            {
                doctormembraincomContext db = new doctormembraincomContext();
                Wiki w = new Wiki();
                List<Wiki> wikis = db.Wiki.ToList();
                ViewBag.Wikis = wikis;
                if (wiki_id == null)
                {
                    w.Chapter = new List<Chapter>();
                }
                else 
                {
                    w = db.Wiki
                        .Include(x => x.Chapter)
                        .Where(x => x.Id == wiki_id)
                        .FirstOrDefault();
                    w.Chapter = w.Chapter.OrderBy(x => x.Priority).ToList();
                }

                return View("CreateWiki", w);
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        public ActionResult CreateWikiPost(Wiki model)
        {
            try
            {
                doctormembraincomContext db = new doctormembraincomContext();
                Wiki w = db.Wiki.Where(x => x.Id == model.Id).FirstOrDefault();
                if (w != null)
                {
                    bool ok;
                    w.Title = StringHelper.OnlyAlphanumeric(model.Title, false, true, "", new char[] { ' ', ',', '-', '\'', '"', '&', '#', '(', ')', '\\', '?', '/', '<', '>' }, out ok);
                    w.Intro = StringHelper.OnlyAlphanumeric(model.Intro, true, true, "br", new char[] { ' ', '.', ',', '-', '\'', '"', '*', '&', '#', '(', ')', '\\', '?', '/', '<', '>' }, out ok);
                }
                else
                {
                    w = new Wiki();
                    bool ok;
                    w.Title = StringHelper.OnlyAlphanumeric(model.Title, false, true, "", new char[] { ' ', ',', '-', '\'', '"', '&', '#', '(', ')', '\\', '?', '/', '<', '>' }, out ok);
                    w.Intro = StringHelper.OnlyAlphanumeric(model.Intro, true, true, "br", new char[] { ' ', '.', ',', '-', '\'', '"', '*', '&', '#', '(', ')', '\\', '?', '/', '<', '>' }, out ok);
                    db.Wiki.Add(w);
                }
                db.SaveChanges();

                return RedirectToRoute("create_wiki", new { wiki_id = w.Id });
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }
        
        public ActionResult RemoveChapterPost(int chapter_id)
        {
            try
            {
                doctormembraincomContext db = new doctormembraincomContext();
                Chapter c = db.Chapter.Where(x => x.Id == chapter_id).FirstOrDefault();
                if (c != null)
                {
                    Wiki w = db.Wiki
                        .Include(x => x.Chapter)
                        .Where(x => x.Id == c.WikiId)
                        .FirstOrDefault();
                    w.Chapter = w.Chapter.OrderBy(x => x.Priority).ToList();
                    foreach (Chapter ch in w.Chapter)
                    {
                        if (ch.Priority > c.Priority)
                            ch.Priority--;
                    }
                    db.Chapter.Remove(c);
                    db.SaveChanges();
                }

                return RedirectToRoute("create_wiki", new { wiki_id = c.WikiId });
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }
        [HttpPost]
        public ActionResult EditChapterPost(int chapter_id, string c_title, string c_text)
        {
            try
            {
                doctormembraincomContext db = new doctormembraincomContext();
                Chapter c = db.Chapter.Where(x => x.Id == chapter_id).FirstOrDefault();
                if (c != null)
                {
                    bool ok;
                    c.Title = StringHelper.OnlyAlphanumeric(c_title, false, true, "", new char[] { ' ', ',', '-', '\'', '"', '&', '#', '(', ')', '\\', '?', '/', '<', '>' }, out ok);
                    c.Text = StringHelper.OnlyAlphanumeric(c_text, true, true, "br", new char[] { ' ', '.', ',', '-', '\'', '"', '*', '&', '#', '(', ')', '\\', '?', '/', '<', '>' }, out ok);
                    db.SaveChanges();
                }

                return RedirectToRoute("create_wiki", new { wiki_id = c.WikiId });
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }
        [HttpPost]
        public ActionResult AddChapterPost(int wiki_id, string c_title, string c_text)
        {
            try
            {
                doctormembraincomContext db = new doctormembraincomContext();
                Wiki w = db.Wiki
                    .Include(x=>x.Chapter)
                    .Where(x => x.Id == wiki_id)
                    .FirstOrDefault();
                if (w != null)
                {
                    bool ok;
                    Chapter c = new Chapter();
                    c.Priority = w.Chapter.Count() + 1;
                    c.Title = StringHelper.OnlyAlphanumeric(c_title, false, true, "", new char[] { ' ', ',', '-', '\'', '"', '&', '#', '(', ')', '\\', '?', '/', '<', '>' }, out ok);
                    c.Text = StringHelper.OnlyAlphanumeric(c_text, true, true, "br", new char[] { ' ', '.', ',', '-', '\'', '"', '*', '&', '#', '(', ')', '\\', '?', '/', '<', '>' }, out ok);
                    w.Chapter.Add(c);
                    db.SaveChanges();
                }

                return RedirectToRoute("create_wiki", new { wiki_id = w.Id });
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }
        public ActionResult MoveChapter(int chapter_id, string dir)
        {
            try
            {
                int? id = null;
                doctormembraincomContext db = new doctormembraincomContext();
                Chapter c = db.Chapter.Where(x => x.Id == chapter_id).FirstOrDefault();
                if(c != null)
                {
                    Wiki w = db.Wiki
                        .Include(x=>x.Chapter)
                        .Where(x => x.Id == c.WikiId)
                        .FirstOrDefault();
                
                    if (w != null)
                    {
                        bool update = true;
                        int new_prio = dir == "up" ? c.Priority - 1 : c.Priority + 1;
                        if (new_prio <= 0)
                            update = false;
                        if (new_prio > w.Chapter.Count())
                            update = false;

                        if(update)
                        {
                            c.Priority = new_prio;
                            foreach (Chapter ch in w.Chapter)
                            {
                                if (ch.Priority == new_prio && ch.Id != c.Id)
                                    ch.Priority = dir == "up" ? ch.Priority + 1 : ch.Priority - 1;
                            }                    
                            db.SaveChanges();
                        }
                        id = w.Id;
                    }
                }

                return RedirectToRoute("create_wiki", new { wiki_id = id });
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }

        [Route("welcome")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            try
            {
                session.Access.Queue();

                string ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
                Stats stats_res = stats.GetStatistics(ip);

                string msg;
                if (ip == Settings.Basic.IP() || Check.Generel.IsAdmin(ip))
                    msg = "Der har været en besøgende! - Admin[" + stats_res.users_per_day + "]";
                else
                    msg = "Der har været en besøgende! - Other[" + stats_res.users_per_day + "]";

                string subject = msg;
                string body = "IP: " + stats_res.ip + "<br />";
                Admin.Notification.Run(Settings.Basic.EMAIL_MAIL(), Settings.Basic.EMAIL_MAIL(), Settings.Basic.EMAIL_MAIL(), subject, body);

                StorieHandler.Setup();

                return View();
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
            finally 
            {
                try
                {
                    session.Access.UnQueue();
                }
                catch (Exception e) 
                {
                    ;
                }
            }
        }

        [Route("guestbook")]
        [AllowAnonymous]
        public IActionResult GuestBook()
        {
            try
            {
                Entry ent = new Entry();
                List<Entry> entrys = new GuestBook().GetEntrys();
                return View(new GuestBook_Dto() { entrys = entrys, message = ent });
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult GuestBookPost(/*GuestBook_Dto model*/string date, string country, string name, string content)
        {
            try
            {
                if (!IsValidEntry(date, country, name, content))
                    throw new Exception();

                bool ok;
                DateTime res_date;
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                DateTimeStyles styles = DateTimeStyles.None;

                DateTime.TryParse(date, culture, styles, out res_date);
                Entry model = new Entry() { Date = res_date.ToString("MM/dd/yyyy"), Country = country, Name = name, Content = content };
                new GuestBook().AddEntry(model);

                string ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
                Stats stats_res = stats.GetStatistics(ip);

                string msg;
                if (ip == Settings.Basic.IP() || Check.Generel.IsAdmin(ip))
                    msg = "Gæstebog(Nyt indlæg)! - Admin[" + stats_res.users_per_day + "]";
                else
                    msg = "Gæstebog(Nyt indlæg)! - Other[" + stats_res.users_per_day + "]";

                string subject = msg;
                string body = "IP: " + stats_res.ip + "<br />" +
                            "date: " + date + "<br />" +
                            "name: " + name + "<br />" +
                            "country: " + country + "<br />" +
                            "content: " + content + "<br />";
                Admin.Notification.Run(Settings.Basic.EMAIL_ADMIN(), Settings.Basic.EMAIL_ADMIN(), Settings.Basic.EMAIL_ADMIN(), subject, body);

                return RedirectToAction("GuestBook");
            }
            catch (Exception e)
            {
                return NotFound(HttpStatusCode.NotFound);
            }
        }

        private bool NotValid(string date, string country, string name, string content) 
        {
            try
            {
                string ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
                Stats stats_res = stats.GetStatistics(ip);

                string msg;
                if (ip == Settings.Basic.IP() || Check.Generel.IsAdmin(ip))
                    msg = "Gæstebog(Ugyldig)! - Admin[" + stats_res.users_per_day + "]";
                else
                    msg = "Gæstebog(Ugyldig)! - Other[" + stats_res.users_per_day + "]";

                string subject = msg;
                string body = "IP: " + stats_res.ip + "<br />" +
                            "date: " + date + "<br />" +
                            "name: " + name + "<br />" +
                            "country: " + country + "<br />" +
                            "content: " + content + "<br />";
                Admin.Notification.Run(Settings.Basic.EMAIL_MAIL(), Settings.Basic.EMAIL_MAIL(), Settings.Basic.EMAIL_MAIL(), subject, body);

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool IsValidEntry(string date, string country, string name, string content)
        {
            try
            {
                bool ok1 = true, ok2 = true, ok3 = true, ok4 = true;

                if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(country) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(content))
                    return NotValid(date, country, name, content);
                if (date.Length > 10 || country.Length > 50 || name.Length > 50 || content.Length > 150)
                    return NotValid(date, country, name, content);

                DateTime res_date;
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                DateTimeStyles styles = DateTimeStyles.None;
         
                ok1 = DateTime.TryParse(date, culture, styles, out res_date);
                StringHelper.OnlyAlphanumeric(country, true, true, "", new char[0], out ok2);
                StringHelper.OnlyAlphanumeric(name, true, true, "", new char[0], out ok3);
                StringHelper.OnlyAlphanumeric(content, true, true, "", new char[] { ' ', '.', ',', ':', ';', '-', '(', ')', '?', '!', '\"', '\'' }, out ok4);
                if (!(ok1 && ok2 && ok3 && ok4))
                    return NotValid(date, country, name, content);
            
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult CheckEntry(string date, string country, string name, string content)
        {
            try
            {
                if (!IsValidEntry(date, country, name, content))
                    return Json(new { success = false });
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult StorieRand()
        {
            try
            {
                Storie s = sh.StorieRand();
                int showing = s.Showing;
                int count = s.Count;
                string header = s.Header;
                string headline = s.HeadLine;
                string content = s.Content;
                return Json(new { success = true, header = header, headline = headline, content = content, showing = showing, count = count });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult StorieLove()
        {
            try
            {
                Storie s = sh.StorieLove();
                int showing = s.Showing;
                int count = s.Count;
                string header = s.Header;
                string headline = s.HeadLine;
                string content = s.Content;
                return Json(new { success = true, header = header, headline = headline, content = content, showing = showing, count = count });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult StorieTop5()
        {
            try
            {
                Storie s = sh.StorieTop5();
                int showing = s.Showing;
                int count = s.Count;
                string header = s.Header;
                string headline = s.HeadLine;
                string content = s.Content;
                return Json(new { success = true, header = header, headline = headline, content = content, showing = showing, count = count });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult StorieFeature()
        {
            try
            {
                Storie s = sh.StorieFeature();
                int showing = s.Showing;
                int count = s.Count;
                string header = s.Header;
                string headline = s.HeadLine;
                string content = s.Content;
                return Json(new { success = true, header = header, headline = headline, content = content, showing = showing, count = count });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }
        //public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        //{
        //    Random rnd = new Random();
        //    return source.OrderBy<T, int>((item) => rnd.Next());
        //}

        [HttpPost]
        public JsonResult AddLink(string suggestion)
        {
            //SetupCurrentUser();

            try
            {
                doctormembraincomContext db = new doctormembraincomContext();
                bool ok;
                suggestion = StringHelper.OnlyAlphanumeric(suggestion.ToLower().Trim(), false, false, "", new char[] { ' ' }, out ok);
                
                if(ok)
                {
                    Wiki w = db.Wiki.Where(x => x.Title.ToLower().Trim().Contains(suggestion)).FirstOrDefault();   

                    if (w != null)
                        return Json(new { success = true, id = w.Id, title = w.Title });
                    else
                        return Json(new { success = false });
                }
                else
                    return Json(new { success = false });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }
    }
}