using Đồ_án_của_Thái.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Đồ_án_của_Thái.ViewModels
{
    public class ComicViewModel
    {
        public int Comic {  get; set; }
        public IEnumerable<Comic> Comics { get; set; }
        public int Trang { get; set; }
        public string PictureChap { get; set; }
        
    }
}