using Đồ_án_của_Thái.DTOs;
using Đồ_án_của_Thái.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Web.Http;

namespace Đồ_án_của_Thái.Controllers
{
    public class FollowsController : ApiController
    {
        private ApplicationDbContext _dbContext;
        public FollowsController()
        {
            _dbContext = new ApplicationDbContext();
        }
        [HttpPost]
        [Authorize]
        public IHttpActionResult Follow(FollowDto followDto)
        {

            var userId = User.Identity.GetUserId();
            var follow = new Follow
            {
                ComicId = followDto.ComicId,
                FolloweeId = userId
            };
            Follow find = _dbContext.Follows.FirstOrDefault(f => f.FolloweeId == userId && f.ComicId == followDto.ComicId);
            if(find == null)
            {
                _dbContext.Follows.Add(follow);
            }
            else
            {
                _dbContext.Follows.Remove(find);
            }
            
            _dbContext.SaveChanges();
            return Json(new { success = true });
        }
    }
}
