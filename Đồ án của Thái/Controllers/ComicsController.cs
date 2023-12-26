using Đồ_án_của_Thái.Models;
using Đồ_án_của_Thái.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

namespace Đồ_án_của_Thái.Controllers
{
    public class ComicsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public ComicsController()
        {
            _dbContext = new ApplicationDbContext();
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }
        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult Detail(int id)
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            var comic = _dbContext.Comics.Include(c => c.Category).SingleOrDefault(c => c.Id == id);
            if (comic == null)
            {
                return HttpNotFound();
            }
            var chapters = from l in _dbContext.Chapters
                           select l;
            var chapter = chapters.Where(c => c.ComicId == id).ToList();
            var comments = _dbContext.Comments.OrderByDescending(c => c.Id).Include(c => c.NameUser).Where(c => c.ComicId == id).ToList();
            var viewModel = new ComicDetailViewModel
            {
                Comic = comic,
                Chapters = chapter,
                Comments = comments,
                Url = Url.Action("Read", new { id = comic.Id })
            };
            
            return View(viewModel);
        }
        private int? GetReadingPageFromCookie(int comicId)
        {
            var userId = User.Identity.GetUserId();
            var cookieName = $"ReadingChapter_{userId}_{comicId}";
            var cookie = HttpContext.Request.Cookies.Get(cookieName);

            if (cookie != null)
            {
                int readingPage;
                if (int.TryParse(cookie.Value, out readingPage))
                {
                    return readingPage;
                }
            }

            return null;
        }

        private void SaveReadingPageToCookie(int comicId, int page)
        {
            var userId = User.Identity.GetUserId();
            var cookieName = $"ReadingChapter_{userId}_{comicId}";
            var cookie = new HttpCookie(cookieName, page.ToString());
            cookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Response.Cookies.Add(cookie);
        }
        public ActionResult Read(int id, int? page)
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            int pageSize = 1;
            int pageNumber = (page ?? 1);
            var read = _dbContext.Chapters.Where(c => c.ComicId == id).ToList();
            var pagedChaps = read.ToPagedList(pageNumber, pageSize);
            var userId = User.Identity.GetUserId();
            ViewBag.LoginUser = userId;
            int? readingPage = GetReadingPageFromCookie(id);
            foreach (Chapter i in read)
            {
                Save find = _dbContext.Saves.FirstOrDefault(s => s.NameUserId == userId && s.ChapterId == i.Id);
                if (find == null)
                {
                    i.isShowSave = true;
                }
            }
            if (readingPage == null || readingPage != pageNumber)
            {
                if (pagedChaps.Count > 0)
                {
                    var currentChapter = pagedChaps[0];
                    currentChapter.IncreaseView();
                    _dbContext.SaveChanges();
                }
            }
            SaveReadingPageToCookie(id, pageNumber);
            ViewBag.BookId = id;
            return View(pagedChaps);
        }
        [Authorize]
        public ActionResult Following()
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            var userId = User.Identity.GetUserId();
            var comics = _dbContext.Follows.OrderByDescending(f => f.Id)
                .Where(f => f.FolloweeId == userId)
                .Select(f => f.Comic)
                .Include(f  => f.Comments)
                .Include(f => f.Category)
                .ToList();
            ViewBag.LogUser = userId;
            var averageRatings = new Dictionary<int, double>();

            foreach (Comic comicss in comics)
            {
                var averageRating = _dbContext.Comments
                    .Where(c => c.ComicId == comicss.Id)
                    .Average(c => (double?)c.rating) ?? 0;

                averageRatings.Add(comicss.Id, averageRating);
            }
            foreach (Comic i in comics)
            {
                Follow find = _dbContext.Follows.FirstOrDefault(f => f.FolloweeId == userId && f.ComicId == i.Id);
                if (find == null)
                {
                    i.isShowFollow = true;
                }
            }
            ViewBag.AverageRatings = averageRatings;
            return View(comics);
        }
        [Authorize]
        public ActionResult Saving()
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            var userId = User.Identity.GetUserId();
            var saves = _dbContext.Saves
                .Where(s => s.NameUserId == userId)
                .Select(s => s.Chapters)
                .Include(ch => ch.Comic)
                .ToList();
            return View(saves);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult Comment(int id, ComicDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newComment = new Comment
                {
                    ComicId = id,
                    BinhLuan = model.BinhLuan,
                    rating = model.rating,
                    NameUserId = User.Identity.GetUserId()
                };
                _dbContext.Comments.Add(newComment);
                _dbContext.SaveChanges();
                var updatedComment = _dbContext.Comments
                    .OrderByDescending(c => c.Id)
                    .Include(c => c.NameUser)
                    .FirstOrDefault(c => c.ComicId == id);

                if (updatedComment != null)
                {
                    var comments = _dbContext.Comments
                        .OrderByDescending(c => c.Id)
                        .Include(c => c.NameUser)
                        .Where(c => c.ComicId == id)
                        .ToList();

                    comments.Insert(0, updatedComment);
                    return PartialView("_CommentPartial", updatedComment);
                }
            }
            return RedirectToAction("Detail", "Comics", new { id = id });
        }
        public ActionResult TheLoai(int? id)
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            var loginUser = User.Identity.GetUserId();
            var comic = _dbContext.Comics.Include(c => c.Category)
                .Include(c => c.Chapters)
                .Include(c => c.Comments)
                .Where(c => c.CategoryId == id)
                .ToList();
            var averageRatings = new Dictionary<int, double>();

            foreach (Comic comicss in comic)
            {
                var averageRating = _dbContext.Comments
                    .Where(c => c.ComicId == comicss.Id)
                    .Average(c => (double?)c.rating) ?? 0;

                averageRatings.Add(comicss.Id, averageRating);
            }
            ViewBag.LogiUser = loginUser;
            foreach (Comic i in comic)
            {
                Follow find = _dbContext.Follows.FirstOrDefault(f => f.FolloweeId == loginUser && f.ComicId == i.Id);
                if (find == null)
                {
                    i.isShowFollow = true;
                }
            }
            ViewBag.AverageRatings = averageRatings;
            return View(comic);
        }
        public ActionResult RankingWeek()
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            var comics = _dbContext.Comics.Include(c => c.Category).Include(c => c.Chapters).Include(c => c.Comments).OrderByDescending(c => c.WeekView).ToList();
            return View(comics);
        }
        [Authorize]
        [HttpGet]
        public ActionResult Profile()
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            var user = User.Identity.GetUserId();
            var profile = _dbContext.Users.FirstOrDefault(u => u.Id == user);
            return View(profile);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAvatar(string imgbase64, string Name)
        {
            var userId = User.Identity.GetUserId();
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (!string.IsNullOrEmpty(imgbase64))
            {
                string imagePath = "/Content/images/" + Guid.NewGuid() + ".png";
                string physicalPath = Server.MapPath(imagePath);
                byte[] bytes = Convert.FromBase64String(imgbase64.Split(',')[1]);
                FileStream stream = new FileStream(physicalPath, FileMode.Create);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                user.Avatar = imagePath;
            }
            else
            {
                user.Avatar = user.Avatar;
            }

            if (!string.IsNullOrEmpty(Name))
            {
                user.Name = Name;
            }
            else
            {
                user.Name = user.Name;
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Profile");
        }
    }
}