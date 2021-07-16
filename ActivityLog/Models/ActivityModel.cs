using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ActivityLog.Models
{
    public class ActivityModel
    {
        public int Id { get; set; }
        public string Log { get; set; }
        public DateTime dateTime { get; set; }
        public virtual int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserModel UserModels { get; set; }
    }
}