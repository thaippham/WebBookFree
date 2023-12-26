using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Đồ_án_của_Thái.Models
{
    public class Save
    {
        public int Id { get; set; }
        public Chapter Chapters { get; set; }
        public int ChapterId { get; set; }
        public ApplicationUser NameUser { get; set; }
        public string NameUserId { get; set; }
    }
}