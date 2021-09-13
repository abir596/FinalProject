using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Models
{
    public class Post
    {
        [Key]
        public int post_id { get; set; }
        [Display(Name = "Post")]
        public string post_txt { get; set; }
        [Display(Name = "Date")]
        public DateTime post_date { get; set; }
        [Display(Name = "Likes")]
        public int post_like { get; set; }
        public int AccountID { get; set; }
    }
}
