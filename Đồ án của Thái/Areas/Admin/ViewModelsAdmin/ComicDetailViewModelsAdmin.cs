using Đồ_án_của_Thái.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Đồ_án_của_Thái.Areas.Admin.ViewModelsAdmin
{
    public class ComicDetailViewModelsAdmin
    {
        public Comic Comic { get; set; }
        public List<Chapter> Chapters { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> Comments { get; set; }
        public string BinhLuan { get; set; }
        public string Url { get; set; }
        public int rating { get; set; }
    }
}