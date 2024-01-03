using Đồ_án_của_Thái.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Đồ_án_của_Thái.Areas.Admin.ViewModelsAdmin;
using PagedList;
using System.Data.Entity;
using System.Web.Security;
using System.Diagnostics;
using System.Web.UI;


namespace Đồ_án_của_Thái.Areas.Admin.Controllers
{

    public class ComicsAdminController : Controller
    {
        private readonly ApplicationDbContext context;
        public ComicsAdminController()
        {
            context = new ApplicationDbContext();
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            var categorys = context.Categories.ToList();
            ViewBag.Categories = categorys;
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }
        
        [Authorize]
        public ActionResult BookPages()
        {
            var comics = context.Comics.ToList();
            return View(comics);
        }
        [Authorize]
        public ActionResult AccountUser()
        {
            var usernonRole = context.Users
                .Where(user => !context.Roles.Any(role => role.Name == user.Name || user.Name == "ADMIN"))
                .ToList();

            return View(usernonRole);
        }
        [Authorize]
        public ActionResult AccountRole()
        {
            var usernonRole = context.Users
                .Where(user => context.Roles.Any(role => role.Name == user.Name && role.Name != "ADMIN"))
                .ToList();
            return View(usernonRole);
        }
        [Authorize]
        public ActionResult CreateAdmin()
        {
            var viewModel = new ComicViewModelsAdmin
            {
                Categories = context.Categories.ToList()
            };
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdmin(ComicViewModelsAdmin comics)
        {
            var comic = new Comic
            {
                NameComic = comics.NameComic,
                Author = comics.Author,
                CategoryId = comics.Category,
                Title = comics.Title,
                Picture = comics.Picture,
                WeekView = 0,
                DateWeekView = UpdateWeekView(),
            };
            context.Comics.Add(comic);
            context.SaveChanges();
            return RedirectToAction("IndexAdmin", "HomeAdmin");
        }
        public DateTime UpdateWeekView()
        {
            var books = context.Comics.Include(b => b.Chapters).ToList();
            DateTime maxDateWeekView = DateTime.MinValue;

            foreach (var book in books)
            {
                if (book.DateWeekView > maxDateWeekView)
                {
                    maxDateWeekView = book.DateWeekView.Value;
                }
            }
            return maxDateWeekView;
        }
        public ActionResult EditAdmin(int id)
        {
            var categories = context.Categories.ToList();
            ViewBag.Categories = categories;
            ViewBag.Categori = new SelectList(categories, "Id", "Name");
            var comic = context.Comics.SingleOrDefault(c => c.Id == id);
            ViewBag.SelectedCategory = comic.CategoryId;
            return View(comic);
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditAdmin(Comic comics)
        {
            var comic = context.Comics.SingleOrDefault(c => c.Id == comics.Id);
            if (!string.IsNullOrEmpty(comics.NameComic))
            {
                comic.NameComic = comics.NameComic;
            }
            else
            {
                comic.NameComic = comic.NameComic;
            }
            if (!string.IsNullOrEmpty(comics.Author))
            {
                comic.Author = comics.Author;
            }
            else
            {
                comic.Author = comic.Author;
            }
            if (comics.CategoryId != 0)
            {
                comic.CategoryId = comics.CategoryId;
            }
            else
            {
                comic.CategoryId = comic.CategoryId;
            }
            if (!string.IsNullOrEmpty(comics.Title))
            {
                comic.Title = comics.Title;
            }
            else
            {
                comic.Title = comic.Title;
            }
            if (!string.IsNullOrEmpty(comics.Picture))
            {
                comic.Picture = comics.Picture;
            }
            else
            {
                comic.Picture = comic.Picture;
            }
            context.SaveChanges();
            return RedirectToAction("IndexAdmin", "HomeAdmin");
        }
        [Authorize]
        public ActionResult DeleteAdmin(int id)
        {
            var categorys = context.Categories.ToList();
            ViewBag.Categories = categorys;
            Comic comics = context.Comics.Find(id);
            if (comics == null)
            {
                return HttpNotFound();
            }
            return View(comics);
        }


        [HttpPost, ActionName("DeleteAdmin")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int id)
        {
            var categorys = context.Categories.ToList();
            ViewBag.Categories = categorys;
            using (var context = new ApplicationDbContext())
            {
                Comic comics = context.Comics.Find(id);
                var followedAccounts = context.Follows.Where(f => f.ComicId == comics.Id).ToList();
                context.Follows.RemoveRange(followedAccounts);
                context.Comics.Remove(comics);
                context.SaveChanges();
            }
            return RedirectToAction("IndexAdmin", "HomeAdmin");
        }
        public ActionResult DetailAdmin(int id)
        {
            var categorys = context.Categories.ToList();
            ViewBag.Categories = categorys;
            var comic = context.Comics.SingleOrDefault(c => c.Id == id);
            if (comic == null)
            {
                return HttpNotFound();
            }
            var chapters = context.Chapters.Where(c => c.ComicId == id).ToList();
            var comments = context.Comments.Where(c => c.ComicId == id).ToList();
            var viewModelAD = new ComicDetailViewModelsAdmin
            {
                Comic = comic,
                Chapters = chapters,
                Comments = comments,
                Url = Url.Action("ReadAdmin", new { id = comic.Id })
            };
            return View(viewModelAD);
        }
        public ActionResult ReadAdmin(int id, int? page)
        {
            var categorys = context.Categories.ToList();
            ViewBag.Categories = categorys;
            int pageSize = 1;
            int pageNumber = (page ?? 1);
            var read = context.Chapters.Where(c => c.ComicId == id).ToList();
            var pagedChaps = read.ToPagedList(pageNumber, pageSize);
            return View(pagedChaps);
        }
        [Authorize]
        public ActionResult CreateChapterAdmin(int id)
        {
            var comic = context.Comics.SingleOrDefault(c => c.Id == id);
            int numberOfChapters = context.Chapters.Count(c => c.ComicId == id);
            var chapter = new Chapter
            {
                Comic = comic,
                Trang = numberOfChapters + 1
            };
            return View(chapter);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChapterAdmin(Chapter viewChap, int id)
        {
            var comics = context.Comics.SingleOrDefault(c => c.Id == id);
            var chapter = new Chapter
            {
                ComicId = comics.Id,
                Trang = viewChap.Trang,
                PictureChap = viewChap.PictureChap,
                View = 0,
            };
            context.Chapters.Add(chapter);
            context.SaveChanges();

            return RedirectToAction("IndexAdmin", "HomeAdmin");
        }
        [Authorize]
        public ActionResult CategoryAdmin()
        {
            var categorys = context.Categories.ToList();
            ViewBag.Categories = categorys;
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryAdmin(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var category = new Category
            {
                Name = model.Name,
            };
            context.Categories.Add(category);
            context.SaveChanges();
            return RedirectToAction("CategoryAdmin", "ComicsAdmin");
        }
        public ActionResult TheLoaiAdmin(int? id)
        {
            var category = context.Categories.ToList();
            ViewBag.Categories = category;
            var comic = context.Comics.Include(c => c.Category).Where(c => c.CategoryId == id).ToList();

            return View(comic);
        }
        public ActionResult ListCategory()
        {
            var category = context.Categories.ToList();
            return View(category);
        }
        public ActionResult EditChapterAdmin(int id)
        {
            var chapter = context.Chapters.SingleOrDefault(c => c.Id == id);
            return View(chapter);
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditChapterAdmin(Chapter chapters, int id) { 
            var chapter = context.Chapters.FirstOrDefault(c => c.Id == chapters.Id);
            var comics = context.Comics.FirstOrDefault(c => c.Id == id);
            chapter.ComicId = chapter.ComicId;
            chapter.Trang = chapters.Trang;
            chapter.PictureChap = chapters.PictureChap;
            context.SaveChanges();
            return RedirectToAction("ReadAdmin", new { id = chapter.ComicId});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccountUserConfirm(string id)
        {
            var user = context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var followedAccounts = context.Follows.Where(f => f.FolloweeId == user.Id).ToList();
            context.Follows.RemoveRange(followedAccounts);
            var SaveAccount = context.Saves.Where(f => f.NameUserId == user.Id).ToList();
            context.Saves.RemoveRange(SaveAccount);
            context.Users.Remove(user);
            context.SaveChanges();
            return RedirectToAction("AccountUser", "ComicsAdmin");
        }
        [HttpPost]
        public ActionResult EditCategory(int id, string Categorynew)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);
            if(category == null)
            {
                return HttpNotFound();
            }
            if(!string.IsNullOrEmpty(Categorynew))
            {
                category.Name = Categorynew;
                context.SaveChanges();
            }
            else
            {
                return RedirectToAction("ListCategory", "ComicsAdmin");
            }
            return RedirectToAction("ListCategory", "ComicsAdmin");
        }
    }
}