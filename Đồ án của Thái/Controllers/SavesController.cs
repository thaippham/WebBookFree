using Đồ_án_của_Thái.DTOs;
using Đồ_án_của_Thái.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Đồ_án_của_Thái.Controllers
{
    public class SavesController : ApiController
    {
        private ApplicationDbContext _dbContext;
        public SavesController()
        {
            _dbContext = new ApplicationDbContext();
        }
        [HttpPost]
        [Authorize]
        public IHttpActionResult Save(SaveDto saveDto)
        {
            var userId = User.Identity.GetUserId();
            var save = new Save
            {
                ChapterId = saveDto.ChapterId,
                NameUserId = userId
            };
            Save find = _dbContext.Saves.FirstOrDefault(s => s.NameUserId == userId && s.ChapterId == saveDto.ChapterId);
            if(find == null)
            {
                _dbContext.Saves.Add(save);
            }
            else
            {
                _dbContext.Saves.Remove(find);
            }
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
