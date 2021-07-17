using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ActivityLog.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar")]
        public string Name { get; set; }
        public virtual ICollection<NewsModel> NewsModels { get; set; }
    }
}