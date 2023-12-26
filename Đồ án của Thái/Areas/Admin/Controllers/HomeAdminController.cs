using Đồ_án_của_Thái.Areas.Admin.Models;
using Đồ_án_của_Thái.Areas.Admin.ViewModelsAdmin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Configuration;
using System.Web.Security;

namespace Đồ_án_của_Thái.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        private Models.ApplicationDbContext context;
        public HomeAdminController()
        {
            context = new Models.ApplicationDbContext();
        }
        // GET: Admin/HomeAdmin
        [Authorize(Roles = "Admin,Manager,Employee")]
        public ActionResult IndexAdmin(string searchString)
        {
            var categorys = context.Categories.ToList();
            ViewBag.Categories = categorys;
            var comic = context.Comics.Include(c => c.Category).ToList();

            var loginUser = User.Identity.GetUserId();
            ViewBag.LoginUser = loginUser;
            var comics = from l in context.Comics
                         select l;
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                comics = comics.Where(c => c.NameComic.ToLower().Contains(searchString));
            }

            return View(comics);
        }
    }
}