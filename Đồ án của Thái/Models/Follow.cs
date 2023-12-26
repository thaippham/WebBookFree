using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Đồ_án_của_Thái.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public Comic Comic { get; set; }
        
        public int ComicId { get; set; }
        public ApplicationUser Followee { get; set; }
        
        public string FolloweeId { get; set; }
    }
}