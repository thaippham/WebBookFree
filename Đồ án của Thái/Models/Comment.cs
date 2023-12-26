using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Đồ_án_của_Thái.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public Comic Comic { get; set; }
        [Required]
        public int ComicId { get; set; }
        public ApplicationUser NameUser { get; set; }
        [Required]
        public string NameUserId { get; set; }
        [Required]
        [StringLength(400)]
        public string BinhLuan { get; set; }
        [Required]
        public int rating { get; set; }

    }
}