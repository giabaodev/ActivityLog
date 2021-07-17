using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ActivityLog.Models
{
    public class NewsModel
    {
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "nvarchar")]
        public string Title { get; set; }
        [Required]
        [Column(TypeName = "nvarchar")]
        public string Noidung { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public virtual Category category { get; set; }
    }
}