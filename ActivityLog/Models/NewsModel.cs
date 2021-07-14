using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ActivityLog.Models
{
    public class NewsModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Noidung { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public virtual Category category { get; set; }
    }
}