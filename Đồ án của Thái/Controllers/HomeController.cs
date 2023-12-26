using Đồ_án_của_Thái.DTOs;
using Đồ_án_của_Thái.Models;
using Đồ_án_của_Thái.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using PagedList.Mvc;
using System.Data.SqlClient;
using Hangfire;

namespace Đồ_án_của_Thái.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _dbContext;
        public HomeController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public ActionResult Index(string searchString, int? page, string sortOrder)
        {
            var comic = _dbContext.Comics.Include(c => c.Category).Include(c => c.Chapters).Include(c => c.Comments).ToList();
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            var loginUser = User.Identity.GetUserId();
            ViewBag.LoginUser = loginUser;
            var averageRatings = new Dictionary<int, double>(); 

            foreach (Comic comicss in comic)
            {
                var averageRating = _dbContext.Comments
                    .Where(c => c.ComicId == comicss.Id)
                    .Average(c => (double?)c.rating)??0;

                averageRatings.Add(comicss.Id, averageRating);
            }
            foreach (Comic i in comic)
            {
                Follow find = _dbContext.Follows.FirstOrDefault(f => f.FolloweeId == loginUser && f.ComicId == i.Id);
                if (find == null)
                {
                    i.isShowFollow = true;
                }
            }

            var comics = from c in comic
                         where string.IsNullOrEmpty(searchString) ||
                               c.NameComic.ToLower().Contains(searchString.ToLower()) ||
                               c.Author.ToLower().Contains(searchString.ToLower())
                         select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                comics = comics.Where(c => c.NameComic.ToLower().Contains(searchString) || c.Author.ToLower().Contains(searchString));

            }
            switch (sortOrder)
            {
                case "Views":
                    comics = comics.OrderByDescending(c => c.Chapters.Sum(chap => chap.View));
                    break;
                case "Rating":
                    comics = comics.OrderByDescending(c => averageRatings[c.Id]);
                    break;
                case "AZ":
                    comics = comics.OrderBy(c => c.NameComic);
                    break;
                case "ZA":
                    comics = comics.OrderByDescending(c => c.NameComic);
                    break;
                default:
                    comics = comics.OrderByDescending(c => c.Id);
                    break;
            }

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            var pagedComics = comics.ToPagedList(pageNumber, pageSize);
            ViewBag.AverageRatings = averageRatings;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchString = searchString;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ComicListPartial", pagedComics);
            }
            RecurringJob.AddOrUpdate("UpdateWeekView", () => UpdateWeekView(), Cron.Daily);
            return View(pagedComics);
        }
        public void UpdateWeekView()
        {
            var books = _dbContext.Comics.Include(b => b.Chapters).ToList();

            foreach (var book in books)
            {
                int totalWeekView = book.Chapters.Sum(c => c.View);
                if (book.DateWeekView.HasValue && (DateTime.Now - book.DateWeekView.Value).TotalDays >= 7)
                {
                    book.WeekView = totalWeekView - book.WeekView;
                    book.DateWeekView = DateTime.Now;
                }
            }
            _dbContext.SaveChanges();
        }
        public ActionResult About()
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            var category = _dbContext.Categories.ToList();
            ViewBag.Categories = category;
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}