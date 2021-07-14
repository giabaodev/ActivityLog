using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ActivityLog.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Please enter the correct password")]
        public string Confirm { get; set; }
        [Required]
        public string Hoten { get; set; }
        public virtual ICollection<ActivityModel> ActivityModels { get; set; }
    }
}