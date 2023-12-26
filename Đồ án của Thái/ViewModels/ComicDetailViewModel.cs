using Đồ_án_của_Thái.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Đồ_án_của_Thái.ViewModels
{
    public class ComicDetailViewModel
    {
        public Comic Comic { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<Comic> ComicList { get; set; }
        public List<Chapter> Chapters { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> Comments { get; set; }
        public int ComicId { get; set; }
        public string BinhLuan { get; set; }
        public string Url { get; set; }
        public int rating { get; set; }
    }
}